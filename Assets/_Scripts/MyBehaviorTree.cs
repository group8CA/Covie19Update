using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class MyBehaviorTree : MonoBehaviour
{
	public Transform[] wander;
	public GameObject[] agents;
    public InteractionObject can;
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

    protected Node PickUp(GameObject p)
    {
        return new Sequence(
                            p.GetComponent<BehaviorMecanim>().Node_StartInteraction(left, can),
                            new LeafWait(1000),
                            p.GetComponent<BehaviorMecanim>().Node_StopInteraction(left));
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
        Val<Vector3> CharPos = Val.V(() => agents[0].transform.position);
        Val<Vector3> otherChar = Val.V(() => agents[3].transform.position);
        Val<Vector3> random = Val.V(() => agents[4].transform.position);
        return new Sequence(
            this.ST_ApproachAndWait(agents[0], wander[0]),
            agents[0].GetComponent<BehaviorMecanim>().ST_PlayGesture("ROAR", AnimationLayer.Face, 3000),
            this.ST_ApproachAndWait(agents[0], wander[1]),
            agents[0].GetComponent<BehaviorMecanim>().ST_PlayGesture("H_Think", AnimationLayer.Hand, 3000),
            this.ST_ApproachAndWait(agents[0], wander[2]),
            agents[0].GetComponent<BehaviorMecanim>().ST_PlayGesture("ROAR", AnimationLayer.Face, 3000),
            new SequenceParallel(this.ST_ApproachAndWait(agents[0], wander[3]),
            this.ST_ApproachAndWait(agents[3], wander[3])
            ),
            this.ST_ShakeHands(agents[0], agents[3], otherChar, CharPos, can, eff1, eff2),
            //agent3 is now infected

            this.ST_ApproachAndWait(agents[0], wander[4]),


            this.ST_ApproachAndWait(agents[1], wander[0]),
            agents[1].GetComponent<BehaviorMecanim>().ST_PlayGesture("H_Yawn", AnimationLayer.Hand, 3000),
            //at this point we want to change color of this agents t-shirt
            //agent1 is now infected
            this.ST_ApproachAndWait(agents[2], wander[1]),
            agents[2].GetComponent<BehaviorMecanim>().ST_PlayGesture("ROAR", AnimationLayer.Face, 3000),
            //agent2 is now infected
            new SequenceParallel(this.ST_ApproachAndWait(agents[2], wander[3]),
            this.ST_ApproachAndWait(agents[3], wander[2]),
            this.ST_ApproachAndWait(agents[4], wander[2])),
            agents[3].GetComponent<BehaviorMecanim>().Node_OrientTowards(otherChar),
            agents[4].GetComponent<BehaviorMecanim>().Node_OrientTowards(random),
            this.Converse(agents[3], agents[4]),
            agents[4].GetComponent<BehaviorMecanim>().ST_PlayGesture("ROAR", AnimationLayer.Face, 3000),
            this.ST_ApproachAndWait(agents[2], wander[3]),
            //new LeafWait(10000),
            //this.hasCorona(agents[0]),
            this.ST_EndTree()
			//this.Converse(agents[0],agents[1])
		);
	}
}
