using System;
using System.Collections.Generic;
using System.Reflection;
using Fohjin.DDD.Bus.Direct;
using Fohjin.DDD.CommandHandlers;
using Fohjin.DDD.EventStore;
using Fohjin.DDD.EventStore.Storage;
using Microsoft.Practices.ServiceLocation;

namespace Fohjin.DDD.Configuration.Castle
{
    public class RegisterCommandHandlersInMessageRouter
    {
        private static MethodInfo _createPublishActionWrappedInTransactionMethod;
        private static MethodInfo _registerMethod;

        public static void BootStrap()
        {
            new RegisterCommandHandlersInMessageRouter().RegisterRoutes(ServiceLocator.Current.GetInstance<IRouteMessages>() as MessageRouter);
        }

        public void RegisterRoutes(MessageRouter messageRouter)
        {
            _createPublishActionWrappedInTransactionMethod = GetType().GetMethod("CreatePublishActionWrappedInTransaction");
            _registerMethod = messageRouter.GetType().GetMethod("Register");

            var commands = CommandHandlerHelper.GetCommands();
            var commandHandlers = CommandHandlerHelper.GetCommandHandlers();

            foreach (var command in commands)
            {
                IList<Type> commandHandlerTypes;
                if (!commandHandlers.TryGetValue(command, out commandHandlerTypes))
                    throw new Exception(string.Format("No command handlers found for event '{0}'", command.FullName));

                foreach (var commandHandler in commandHandlerTypes)
                {
                    var injectedCommandHandler = GetCorrectlyInjectedCommandHandler(commandHandler);
                    var action = CreateTheProperAction(command, injectedCommandHandler);
                    RegisterTheCreatedActionWithTheMessageRouter(messageRouter, command, action);
                }
            }
        }

        private static object GetCorrectlyInjectedCommandHandler(Type commandHandler)
        {
            return ServiceLocator.Current.GetInstance(commandHandler);
        }

        private static void RegisterTheCreatedActionWithTheMessageRouter(MessageRouter messageRouter, Type commandType, object action)
        {
            _registerMethod.MakeGenericMethod(commandType).Invoke(messageRouter, new[] { action });
        }

        private object CreateTheProperAction(Type commandType, object commandHandler)
        {
            return _createPublishActionWrappedInTransactionMethod.MakeGenericMethod(commandType, commandHandler.GetType()).Invoke(this, new[] { commandHandler });
        }

        public Action<TCommand> CreatePublishActionWrappedInTransaction<TCommand, TCommandHandler>(TCommandHandler commandHandler) 
            where TCommand : class 
            where TCommandHandler : ICommandHandler<TCommand>
        {
            return command => new TransactionHandler<TCommand, TCommandHandler>(
                            ServiceLocator.Current.GetInstance<IEventStoreUnitOfWork<IDomainEvent>>()
                      ).Execute(command, commandHandler);

            //return command => ServiceLocator.Current.GetInstance<TransactionHandler<TCommand, TCommandHandler>>().Execute(command, commandHandler);
        }
    }
}