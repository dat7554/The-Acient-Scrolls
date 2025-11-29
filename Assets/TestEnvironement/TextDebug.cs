using Characters.FSM.EnemyFSM;
using Characters.NPC.Enemy;
using Characters.PlayerSystem;
using PlayerSystem;
using TMPro;
using UnityEngine;

public class TextDebug : MonoBehaviour
{
    [SerializeField] private PlayerCharacter player;
    [SerializeField] private EnemyStateMachine enemyStateMachine;
    
    [SerializeField] private TextMeshProUGUI stanceText;
    [SerializeField] private TextMeshProUGUI groundText;
    [SerializeField] private TextMeshProUGUI onSlopeText;
    [SerializeField] private TextMeshProUGUI verticalVelText;
    [SerializeField] private TextMeshProUGUI horizontalVelText;
    [SerializeField] private TextMeshProUGUI finalMoveText;
    [SerializeField] private TextMeshProUGUI deltaTimeText;
    [SerializeField] private TextMeshProUGUI enemyStateText;

    private void Update()
    {
        stanceText.text = player.PlayerStance.ToString();
        groundText.text = player.IsGrounded ? "On ground" : "On air";
        onSlopeText.text = player.OnSlope ? "On slope: true" : "On slope: false";
        verticalVelText.text = "Vertical: " + player.VerticalVelocity.ToString();
        horizontalVelText.text = "Horizontal: " + player.HorizontalVelocity.ToString();
        finalMoveText.text = "Final: " + player.FinalMovement.ToString();
        deltaTimeText.text = "Delta Time: " + player.DeltaTime.ToString();
        
        enemyStateText.text = "Enemy State: " + enemyStateMachine?.CurrentState.ToString();
    }
}
