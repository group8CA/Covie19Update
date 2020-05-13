using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class MyBehaviorTree : MonoBehaviour
{
	public Transform[] wander;
	public GameObject[] agents;
	public InteractionObject[] obj;
	public FullBodyBipedEffector eff1;
	public FullBodyBipedEffector eff2;
	public FullBodyBipedEffector left;
	private BehaviorAgent behaviorAgent;

	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	

	
	protected Node Converse(GameObject Wanderer, GameObject Friend)

    {

        return

            new DecoratorPrintResult(

                new Sequence( 

                Wanderer.GetComponent<BehaviorMecanim>().ST_PlayGesture("SURPRISED", AnimationLayer.Hand, 3000),

                Friend.GetComponent<BehaviorMecanim>().ST_PlayGesture("WAVE", AnimationLayer.Hand, 3000),

                Wanderer.GetComponent<BehaviorMecanim>().ST_PlayGesture("CLAP", AnimationLayer.Hand, 3000),

                Friend.GetComponent<BehaviorMecanim>().ST_PlayGesture("POINTING", AnimationLayer.Hand, 3000))
				)
				;

    }

	protected Node PickUp(GameObject Friend, Val<InteractionObject> phone){
		return new Sequence(
			Friend.GetComponent<BehaviorMecanim>().Node_StartInteraction(left, phone),
			//Friend.GetComponent<BehaviorMecanim>().Node_StartInteraction(left, obj2),
			new LeafWait(1000),
			Friend.GetComponent<BehaviorMecanim>().ST_PlayBodyGesture("TALKING ON PHONE", 3000),
            Friend.GetComponent<BehaviorMecanim>().Node_StopInteraction(left),
			Friend.GetComponent<BehaviorMecanim>().ST_PlayGesture("POINTING", AnimationLayer.Hand, 3000)
			
			);
		
	}
	
	protected Node ST_ShakeHands( GameObject Friend, GameObject Wanderer, Val<Vector3> WandererPos, Val<Vector3> FriendPos, Val<InteractionObject> obj, Val<FullBodyBipedEffector> effector1, Val<FullBodyBipedEffector> effector2)

    {

        return new Sequence(
			
            Friend.GetComponent<BehaviorMecanim>().Node_OrientTowards(WandererPos),                
			Wanderer.GetComponent<BehaviorMecanim>().Node_OrientTowards(FriendPos),

            Friend.GetComponent<BehaviorMecanim>().Node_StartInteraction(effector1, obj),
			
			new LeafWait(2000),

            Wanderer.GetComponent<BehaviorMecanim>().Node_StartInteraction(effector2, obj),
			
			new LeafWait(2000),
			
			Wanderer.GetComponent<BehaviorMecanim>().Node_StopInteraction(effector2),
			
			Friend.GetComponent<BehaviorMecanim>().Node_StopInteraction(effector1)

            //new LeafWait(1000)
			);

    }
	

	
	protected Node ST_ApproachAndWait(GameObject agents, Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( agents.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(2000));
	}
	
	protected Node ST_EndTree(){
		return new DecoratorLoop (
			new Sequence( new LeafWait(2000))
			);
	}
	protected Node BuildTreeRoot()
	{

		return new Sequence(
			this.ST_ApproachAndWait(agents[0],wander[0]),
			//new LeafWait(10000),
			//this.hasCorona(agents[0]),
			this.ST_EndTree()
			//this.Converse(agents[0],agents[1])
		);
	}
}
