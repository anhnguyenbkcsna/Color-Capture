using Script.Component;
using Unity.Entities;

namespace Script.Systems
{
    public partial struct ChangeTurnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RedPlayerTag>();
            state.RequireForUpdate<GreenPlayerTag>();
        }
        public void OnUpdate(ref SystemState state)
        {
            // var player1 = SystemAPI.GetSingletonEntity<RedPlayerTag>();
            // var player2 = SystemAPI.GetSingletonEntity<GreenPlayerTag>();
            // if (!SystemAPI.HasComponent<PlayerTurnTag>(player1) && !SystemAPI.HasComponent<PlayerTurnTag>(player2))
            // {
            //     // Init
            //     state.EntityManager.AddComponent<PlayerTurnTag>(player1);
            // }
            // // Take turn
            // if (!SystemAPI.HasComponent<PlayerTurnTag>(player1))
            // {
            //     // Wait until perform a move
            // }
            // if (!SystemAPI.HasComponent<PlayerTurnTag>(player2))
            // {
            //     // Wait until perform a move
            //     
            // }
        }
    }
}