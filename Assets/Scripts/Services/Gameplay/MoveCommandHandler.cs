using Assets.Scripts.Services;
using mmo_shared.Messages;
using UnityEngine;

namespace Assets.Scripts.Services {
    class MoveCommandHandler : MonoBehaviour{
        private MessageSender messageSender;
        private InputHandler inputHandler;
        private PlayerService playerService;

        void Awake() {
            messageSender = FindObjectOfType<MessageSender>();
            inputHandler = FindObjectOfType<InputHandler>();
            playerService = FindObjectOfType<PlayerService>();
        }

        void Start() {
            inputHandler.TerrainMoveCommand += MoveTo;
        }

        private void MoveTo(Vector3 point) {
            if (!playerService.MainPlayerId.HasValue) {
                return;
            } else if (!playerService.CanMove(playerService.GetMainPlayer())){
                return;
            }
            messageSender.Send(new MoveCommand() { Target = new mmo_shared.Vector2(point.x, point.z) });
        }
    }
}
