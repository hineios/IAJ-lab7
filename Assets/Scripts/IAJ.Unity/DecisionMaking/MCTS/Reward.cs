namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class Reward
    {
        public float Value { get; set; }
        public int PlayerID { get; set; }

        public float GetRewardForNode(MCTSNode node)
        {
            //this is only used for the initial node
            if(node.Parent == null)
            {
                return this.Value;
            }

            //a reward for a node is given by the player of the node's parent (which is the one taking the decision about what is the best child to choose)
            int player = node.Parent.PlayerID;

            if(player == this.PlayerID)
            {
                return this.Value;
            }
            else
            {
                return 1-this.Value;
            }
        }
    }
}
