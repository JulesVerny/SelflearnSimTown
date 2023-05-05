# Citizens Learn to Live in Simple Town: Rudimentary Self Learning  #

A simple Town Simulation with Citizens behaviour dictated by a Rudimentary Self learning Algorithm. The Citizens behavioural actions are dictated by their current feelings and levels of money and food.  The intended operaiton is for the Self Learning Algorithm to develop a useful representation of which is the next 'Best' Action to take, given the Citezens current feelings. 

The project is implemented in the Unity game engine, with the self learning algorithm implemented within the C# scripts. 

This is inspired by the awareness and frustrations in getting Q learning and ploicy based Reinforcement learning algoirhtms to learn and optimsie behaviour. Here instead a very rudimentary self learning algorithm is set up through a single layer nueral weights multiplication, softmax activation feeding a probability distribution. From which is drawn a probabilistic next Action choice. 

The results suggest some basic learn't behaviours. 

![ScreenShot](OverviewPic.PNG)

See a You Tube Video Here << >>

## Implementaiton Details ##

The basic scene is set up in Unity, with various characters and buildings modelled in Blender and .FBX exported and imported into Unity. The Character animations are downloaded from Mixamo. You can download the Unity Scene and Assets from teh package provided above. 

### Citizen Feelings and Building/Activity Impacts ###

The Citizens Feelings, as implemented in the CitzAttributes.CS are:
-   Tiredness     (0 to 100)
-   Hungriness    (0 to 100)
-   Happiness     (-100 to +100)
-   Money         (0 to 100)
-   Food at Home  (0 to 100)

The Possible Actions that a Citizen can take: 
-   Go To Office
-   Go Food Shopping
-   Go To Pub
-   Go To Resturant To Eat
-   Go To Park
-   Go Home To Eat
-   Go Home To Rest

Each of the Sites (Office, Resturant, Pr etc) holds a Scriptable object configuration, reflecting the Action consequences on each of the Citezens feelings above.   

### Main Script Files ###

The main C# Script files are as follows: 

-   ExperimentManager.CS    :  This is the overall files that intercpes user inputs creates and schedules the simulation time
-   BrainManager.CS         :  This manages the self learning brain. It Maintains the Brain weights matrix. More details below 
-   UserInterfaceController.CS      :  This provides a canvas to display a Citizen overlay display panel 
-   CitzController.CS       :  This is the Citizen controller, it manages the execution and animation of a Citizen
-   CitzAttributes.CS       : This is a simple class container of the current Citizen feelings, money and food levels
-   SiteManager.CS          : This is a wrapper manager for a list of building sites 
-   SiteController.CS       : This is a building controller, it processes the consequences of the Citizens last Action.
-   ProbSet.CS              : This is a contaioner class for a Set of probablities associated with a possible Action Set.
-   OrbitCamera.CS          : A simple Main Camera control for Zoom in and out, and orbit around the Town Centre
-   SiteConfiguration.CS    : A Scriptable object, to hold the configuration parameters associated with different Site impacts

### Main Play ###

The Unity project run time starts directly into the Simulation main scene, under the control of the Experiment Manager. 

After pressing Keyboard C, the Experiment Manager will create a set of Citizens (2x Grandads, 2x Grandmas, 2x Juliets, 2x Thomas, 2x Leo and 2 x Max). It will then schedule their simulation clicks. The Citizens determine their next Action by calling upon the BrainManager.RequestNextAction( CurrentFeelings), and proceeed to walk twoards that site representing their next Action. There is a dwell time within each sit action, to represent the Action activity taking place. The consequence of the Action, upon the Citizens feeling are updated through a call to that buildings, ProcessSiteImpactUponFeeelings() method. The buildings impact is determiend by its specific configurable impact upon a Citizen is executed through the  Upon completion of that Action, the Citizen then calls upon RequestNextAction() again. 

## User Controls ##

The Experiment Manager invokes several keyboard controls:

    -   C Key  :   Create a Set of Citizens. Eight Citezens will be created. 
    -   S Key  :   Save the Current Brain to a File under Assets/Brain
    -   R Key  :   Recover a previously saved Brain File into the main Brain
    -   F Key  :   Speed up the Simulation (Training) by a factor of 5 (Note The Animations are not factored)
    -   N Key  :   Return to Normal Speed Simulation  
    -   Esc Key:   The exit the simulation. 

View controls via the Orbit Camera control:
    - Mouse Wheel, Keyboard Up and Down Arrow to Zoom In/ Out
    - Keyboard Left and Righ Arrow Orbit Left/Right aorund Town Centre

## Rudimentary Self Learning Algorithm ###

Note the following is NOT Mathematically sound or in any way proven to work or optimise. It is an experiment in a simpler approach to self learning. It is based upon experience with nueral networks, pytorch and reinforcement learning techniques. It is a much simpler approach, based upon a single network layer of weights multiplication. 

The core premise is for the Brain to develop a usful representation of which is the next 'Best' Action to take, given the Citizens current feelings. The 'algorithm' is implemented within the BrainManager.CS.  This maintains a set of weights in a 7 rows x 5 columns C# (float) array. This represents the forward pass of the 7 Actions described above (in rows) and the 5 Citzen Feelings.  

### Forward Pass: RequestNextAction(CurrentFeelings) ###

The forward pass, RequestNextAction(CurrentFeelings): Offer a suggested next 'Best' Action given the Current Citizens Feelings. This is a four stage process:
- Calculate Activations through a Matrix Multiplication of the Brain Weights by the Current Citzens Feelings
- Perform a Softmax function on those Activations to Normalise Activations into a Probabilities set each range (0 to 100)
- Impose any Hard Constraints, by Zeroing out any Probablities associated with Un affordable Actions
- Make a Random Action Choice from the Random Distribution proprtioned by the Calculated Probabilities    

Return the selected Random Action Choice.   In principle the single stage nueral network layer will have influenced higher probabilties into teh distribution towards the most favourable actions, and smaller probabilites for the less effective actions. This is the same principle as the classif REINFORCE policy based reinforcement learning algorithms.   

### Good and Bad Rewards  ###

In order to learn 'Good' Behaviours, and hence develop 'Best Actions. we need to define a useful Reward function, to drive the algorithm.  The following Reward fucntions are somewhat arbitary: 

        // Good Positive Rewards
    -   if (TheCurrentCitzFeelings.Hapiness >75) RtnReward = 1.0f;
    -   if ((TheCurrentCitzFeelings.Tiredness < 25) && (TheCurrentCitzFeelings.Hungryness < 25)) RtnReward = 1.0f;
    -   if (TheCurrentCitzFeelings.Money > 75) RtnReward = 1.0f;

        // Bad Negative Rewards
    -   if (TheCurrentCitzFeelings.Money < 10) RtnReward = -1.0f;
    -   if (TheCurrentCitzFeelings.Hapiness < -50) RtnReward = -1.0f;
    -   if ((TheCurrentCitzFeelings.Tiredness > 80) && (TheCurrentCitzFeelings.Hungryness > 80)) RtnReward = -1.0f;

Note the algorithm performance and Citizens subsequence behaviours will ebvery sensitive to the Reward defintion function. As are all Machine Learning algorithms driven by a clear cost/ evaluation function or the Reward fucntion in Renforcement Learning.  

The Reward is passed into the Self learning Updates, Along with the Previous Action and the Consequential Citizens Feelings, as a classic (State,Action, Reward) sequence into Reinforcment Learning Algorithms. (Barto and Sutton)

### Self Learning Update : ReinforceActionWeights()  ###

The Self learning algorithm follows the classic RL algorithm step updates, by processing through the tuple of (Last Action Applied, The Citizens Feelings, as a proxy of State, and the Reward). This is implemented within the ReinforceActionWeights() mehtod of BrainManager.CS.

The self learning method described here is very rudimentary, not mathemically sound nor proven. But instead represents gross simplification of classic back propagation optimisation and deep RL methods. 

Basically each Brain Weight is adjusted by a Delta:

-   DeltaCitzAttributeWeight = LearnigRate * (The Citziens Feeling Attribute -50) * RewardFactor
-   BrainWeight[LastAction] = BrainWeight[LastAction] + DeltaCitzAttributeWeight

The -50 subtraction is to Normalise the weights halfway in the normal [0 to 100] range of each Citz Feeling attribute, except for Hapiness which is left without a subtraction, since that attribute ranges between [-100 and 100]

For all [7 Action x 5 CitZ Feeling ] Weights 

That is fundamentally all there is to this rudimentary self learning algorithm.  

### Temporal Discount ###

There is a slight adjustment to the above, to reflect the general Reinforcment Learning technique to overcome sparse rewards, and the consequence of previous Actions.  In classic RL algorithms, a history, or buffer, of previous [State, Action, Rewards] are maintained, and the reinforce weights updates are applied through the whole sequence by a Gamma discount factor. This then enables the effects of sparse rewards to be reflected back through recognition fo previous actions. 

In this rudimenatry self learning algorithm, each Citizen maintains its own small history buffer of the previous three (Actions Feeling States and Rewards). See the variables named PrevReward, Prev2Reward, PrevFeelings etc. At each Self learning update within the CitzController.ProcessTheImpactUponCitz() method all three sets are applied through the BrainManager.ReinforceActionWeights() calls, with the previous Rewards being discounted at 0.85 and 0.85*0.85 respectively as a pretty course Gamma value. 

In this way the last three Actions, Rewards and Citizen states are used to reinforce the Brain Weights.

## Results ##

The results are quite interesting but are not claimed to be either optimal or correct. The simulation being vey sensitive to the charactrisation of the Action impacts within each of the Building Site configurations, the reward function etc. 

The initial Citizen behaviours, at the start,  choose acros all possible actions in a fairly random manner. So generally running out of money, poor average hapiness etc. So in the beginning they visit across all buildings, with actions being  equally likely. (Including Visits to the Pub)

However the emergent behaviour after about twenty minutes of (Fast) Runtime simulation is that the typical Citizen Action sequence being a preference towards a Sequence of Going to Work (Out of necessity for some funding Money) => Going to Restaurant or Park to increase Happiness and going Home to Rest.  The Citizens seem less inclined to go Food shoping and Eating at Home. I guess because they can meet their Hunger and Happiness needs better by Going Out to Eat. The Citizens rarely went to the Pub, which although high on Hapiness but does not meet their hunger needs as well as the going to Restaurant.    

## Acknowldgements ##

-   Barto and Sutton: Renforcment Learning: An Introduction
-   Morales: Grokking Deep Reinforcement Laerning:  

