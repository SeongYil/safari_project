using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Assets;

namespace Assets.Resources.Scripts
{
    public class SafariAgent : Agent
    {

		private SharedDataType.EColor eColor = SharedDataType.EColor.Count;

		public bool isRandomAgent = false;

		private AnimalRuleManager ruleManager;

		private Unity.MLAgents.Policies.BehaviorParameters behaviorParameters;

		private int AllActionSize = 360;
		private int AllObservationSize = 18;
		

		public void InitializeAgent(AnimalRuleManager animalRulemanager, string name , SharedDataType.EColor colorType)
        {
			ruleManager = animalRulemanager;
			eColor = colorType;
			behaviorParameters.TeamId = (int)colorType;
		}

		public void Awake()
        {
			behaviorParameters = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();
			behaviorParameters.BehaviorName = name;
			behaviorParameters.BrainParameters.VectorObservationSize = AllObservationSize;
			behaviorParameters.BrainParameters.NumStackedVectorObservations = 1;

			int[] brancheSize = new int[1] { AllActionSize };
			ActionSpec acionSpec = new ActionSpec(0, brancheSize);
			behaviorParameters.BrainParameters.ActionSpec = acionSpec;

			behaviorParameters.InferenceDevice = Unity.MLAgents.Policies.InferenceDevice.GPU;

			behaviorParameters.BehaviorType = Unity.MLAgents.Policies.BehaviorType.Default;


			if (name.Contains("Black"))
            {


				behaviorParameters.TeamId = (int)SharedDataType.EColor.Black;
				if (GameManager.instance.ReleaseMode == true)
				{
					behaviorParameters.Model = GameManager.instance.BlackModel;
					if (GameManager.instance.HumanColor == SharedDataType.EColor.Black)
					{
						behaviorParameters.BehaviorType = Unity.MLAgents.Policies.BehaviorType.HeuristicOnly;
					}
				}
				//isRandomAgent = true;
			}
            else
            {
				behaviorParameters.TeamId = (int)SharedDataType.EColor.White;
				if (GameManager.instance.ReleaseMode == true)
                {
					behaviorParameters.Model = GameManager.instance.WhiteModel;
				}

				if (GameManager.instance.HumanColor == SharedDataType.EColor.White)
				{
					behaviorParameters.BehaviorType = Unity.MLAgents.Policies.BehaviorType.HeuristicOnly;
				}
			}


			

		}

        public override void OnEpisodeBegin()
		{
			//ruleManager.ResetGame();
		}

		public override void CollectObservations(VectorSensor sensor)
		{
			int[,] boardState = ruleManager.env.GetCurrentBoardState();
			int[,] stockState = ruleManager.env.GetCurrentStockState();

			foreach( int pieceID in boardState )
            {
				sensor.AddObservation(pieceID);
            }

            foreach (int stockCount in stockState)
            {
                sensor.AddObservation(stockCount);
            }


        }

		public override void OnActionReceived(ActionBuffers actionBuffers)
		{
			

			int action = actionBuffers.DiscreteActions[0];

			//if(isRandomAgent == false)
            //{
			ruleManager.SetActionMove(action);
			//}
   //         else
   //         {
			//	Dictionary<double, double> allAvailableAction = ruleManager.GetAvailableAllActions();

			//	List<KeyValuePair<double,double>> listAvailable = allAvailableAction.ToList();

			//	int randomAction = UnityEngine.Random.Range(0, listAvailable.Count);

			//	ruleManager.SetActionMove(listAvailable[randomAction].Key);

			//}

			


		}

		public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
		{
			//내 턴이 아니면 액션하지 않음
			for (int i = 0; i < AllActionSize; ++i)
			{
				actionMask.SetActionEnabled(0, i, false);
			}


			Dictionary<double, double> allAction = ruleManager.GetAvailableAllActions();

			foreach( KeyValuePair<double, double> availableAction in allAction )
            {
				int index = (int)availableAction.Value;
				actionMask.SetActionEnabled(0, index, true);
			}




		}

		public override void Heuristic(in ActionBuffers actionsOut)
		{



		}

	}
}
