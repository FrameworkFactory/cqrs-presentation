using FWF.Basketball.CQRS.Commands;
using FWF.Basketball.Logic.Data;
using FWF.CQRS;
using FWF.Logging;
using FWF.Security;
using System;

namespace FWF.Basketball.CQRS.CommandHandlers
{
    internal class AddPlayerCommandHandler : ICommandHandler<AddPlayerCommand>
    {
        private readonly IGameDataRepository _gameDataRepository;
        private readonly ILog _log;

        public AddPlayerCommandHandler(
            IGameDataRepository gameDataRepository,
            ILogFactory logFactory
            )
        {
            _gameDataRepository = gameDataRepository;

            _log = logFactory.CreateForType(this);
        }

        public ICommandResponse Handle(ISecurityContext securityContext, AddPlayerCommand command)
        {

            // TODO: Should check ISecurityContext if the user is allowed to run the command

            var player = new Player
            {
                Id = command.Id.GetValueOrDefault(),
                Name = command.Name,
                TeamId = command.TeamId.GetValueOrDefault(),

                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
            };

            try
            {
                using (var writeContext = _gameDataRepository.BeginWrite())
                {
                    writeContext.Insert(player);
                }
            }
            catch(Exception ex)
            {
                _log.Error(ex, ex.Message);

                // Command did not finish correctly...
                return new CommandResponse
                {
                    ErrorCode = 123,
                    ErrorMessage = ex.Message
                };
            }

            return new CommandResponse();
        }

    }
}
