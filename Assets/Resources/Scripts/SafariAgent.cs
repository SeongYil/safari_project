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

		public Unity.MLAgents.Policies.BehaviorParameters behaviorParameters;

		private int AllActionSize = 360;
		private int AllObservationSize = 16 * 4 * 3;

		public void InitializeAgent(AnimalRuleManager animalRulemanager, string name, SharedDataType.EColor colorType)
		{
			ruleManager = animalRulemanager;
			eColor = colorType;

			behaviorParameters = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();
			behaviorParameters.BehaviorName = name;
			behaviorParameters.BrainParameters.VectorObservationSize = AllObservationSize;
			behaviorParameters.BrainParameters.NumStackedVectorObservations = 3;
			behaviorParameters.TeamId = (int)colorType;

			int[] brancheSize = new int[1] { AllActionSize };
			ActionSpec acionSpec = new ActionSpec(0, brancheSize);
			behaviorParameters.BrainParameters.ActionSpec = acionSpec;
			behaviorParameters.InferenceDevice = Unity.MLAgents.Policies.InferenceDevice.Default;
			behaviorParameters.BehaviorType = Unity.MLAgents.Policies.BehaviorType.Default;
			behaviorParameters.DeterministicInference = true;

			if (colorType == SharedDataType.EColor.Black)
			{
				behaviorParameters.Model = GameManager.instance.BlackModel;
			}
			else
			{
				behaviorParameters.Model = GameManager.instance.WhiteModel;
			}

		}

		public void Awake()
		{





		}

        public override void OnEpisodeBegin()
		{




		}

		public override void CollectObservations(VectorSensor sensor)
		{
			int[,] boardState = ruleManager.env.GetCurrentBoardState();
			int[,] stockState = ruleManager.env.GetCurrentStockState();


			int [,,] obs = Decoder.get_observation(boardState, stockState);





			//foreach (int pieceID in boardState)
			//{
			//	sensor.AddObservation(pieceID);
			//}

			//foreach (int stockCount in stockState)
			//{
			//	sensor.AddObservation(stockCount);
			//}

			foreach(int data in obs)
            {
				sensor.AddObservation(data);
			}


		}

		public override void OnActionReceived(ActionBuffers actionBuffers)
		{


			
			int action = actionBuffers.DiscreteActions[0];
			

			if(action == 0)
            {
				//GameManager.instance.eror
            }


			(string start, string dest) pos = Decoder.action_to_stringTuple(action);




			AnimalRuleManager.EGameState actionResult = ruleManager.SetActionMove(pos.start, pos.dest);

			//  1 ,2 ,3
			//a 1a 2a 3a
			//b 1b 2b 3b
			//c 1c 2c 3c
			//d 1d 2d 3d 

			if(GameManager.instance.ReleaseMode == false)
            {
				//학습
				ruleManager.SetReward(actionResult);
			}

            if (actionResult == AnimalRuleManager.EGameState.Win)
            {
				ruleManager.Agent[SharedDataType.EColor.White].EndEpisode();
				ruleManager.Agent[SharedDataType.EColor.Black].EndEpisode();
				ruleManager.EndepiosodeState = true;
                return;
            }

            ruleManager.ChangeTurn();


        }

		public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
		{
			//내 턴이 아니면 액션하지 않음
			for (int i = 0; i < AllActionSize; ++i)
			{
				actionMask.SetActionEnabled(0, i, false);
			}


			Dictionary<double, double> allAction = ruleManager.GetAvailableAllActions();

			foreach (KeyValuePair<double, double> availableAction in allAction)
			{
				int index = (int)availableAction.Value;
				actionMask.SetActionEnabled(0, index, true);
			}




		}


    }
}
