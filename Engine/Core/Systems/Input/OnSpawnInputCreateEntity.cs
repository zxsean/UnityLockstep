﻿using System.Collections.Generic;     
using BEPUutilities;
using Entitas;
using Lockstep.Core.Interfaces;

namespace Lockstep.Core.Systems.Input
{
    public class OnSpawnInputCreateEntity : ReactiveSystem<InputEntity>
    {
        private uint _nextEntityId;

        private readonly IGameService _gameService;
        private readonly GameContext _gameContext; 

        public OnSpawnInputCreateEntity(Contexts contexts, ServiceContainer services) : base(contexts.input)
        {
            _gameService = services.Get<IGameService>();     
            _gameContext = contexts.game;              
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {                                
            return context.CreateCollector(InputMatcher.AllOf(InputMatcher.EntityConfigId, InputMatcher.Coordinate, InputMatcher.PlayerId));
        }

        protected override bool Filter(InputEntity entity)
        {                     
            return entity.hasEntityConfigId && entity.hasCoordinate && entity.hasPlayerId;
        }

        protected override void Execute(List<InputEntity> inputs)
        {
            foreach (var input in inputs)
            {
                var e = _gameContext.CreateEntity();

                e.isNew = true;
                e.AddId(_nextEntityId);
                e.AddOwnerId(input.playerId.value);

                e.AddVelocity(Vector2.Zero);
                e.AddPosition(input.coordinate.value);

                _gameService.LoadEntity(e, input.entityConfigId.value);    
                _nextEntityId++;  
            }                                                                                    
        }    
    }
}
