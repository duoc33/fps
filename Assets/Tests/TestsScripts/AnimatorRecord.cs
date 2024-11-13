using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AnimatorRecord : MonoBehaviour
{
    private Animator animator;
    private int layerIndex;
    private AnimationEvent animationEvent;
    private AnimatorStateInfo currentStateInfo;
    private AnimatorTransitionInfo currentTransitionInfo;
    private AnimatorClipInfo animatorClipInfo;

    private AnimatorOverrideController overrideController;

    private int stateHash;
    public void AnimatorTest()
    {
        // animator 勾选Has Exit Time , Exit Time = 1 , 从最后一帧，Transition Duration = 0 , 可以让状态自动退回。

        // 在FBX的动画文件中，声明的一个动画的Curve值名称，其变化曲线值的值，会在动画片段的属性中自动出现，它也会赋值给动画状态机中同名的属性，
        // 也就是当Animator播放到当前动画时，Aniamtor会读取动画片段中该属性的值，并付给同名的Aniamtor中的参数。
        animator.GetFloat("");

        animator.Play("", layerIndex);
        animator.Play(stateHash,layerIndex,0.4f);//0~1,为0时就重播。
        animator.Play(stateHash);
        animator.Play("");
        animator.Play(stateHash,layerIndex);
        animator.Play("",layerIndex,0.4f);
        animator.StartPlayback();//倒放
        animator.StopPlayback();

        //Sync 只是同步状态，并不会同步，具体内容，它们Weight Sync成往往会慢一点，timing勾选上，则根据Weight大小确定同步程度。

        // 暂停
        animator.speed = 0;
        // 继续播放
        animator.speed = 1;


        //当前动画机播放时长
        float currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;//0~1
        //动画片段长度
        float length = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length; //秒
        //获取动画片段帧频
        float frameRate = animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate; //采样每秒有多少帧 ，秒/帧
        //计算动画片段总帧数
        float totalFrame = length / (1 / frameRate);
        //计算当前播放的动画片段运行至哪一帧
        int currentFrame = (int)(Mathf.Floor(totalFrame * currentTime) % totalFrame); //（clipTime改为currentTime） Debug.Log(" Frame: " + currentFrame + “/” +totalFrame);

        //从某一帧开始播放动画
        void OnPlayAnimatorFromFrame(string stateName, float frame)
        {
            animator.Play(stateName, 0, frame);
            animator.speed = 1;
        }
        OnPlayAnimatorFromFrame("",currentFrame);
    }
    


    public void OverrideController()
    {
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);
        

        List<AnimationClip> newclips = new AnimationClip[overrides.Count].ToList();
        AnimationClip[] clips = overrideController.animationClips;

        foreach (AnimationClip newclip in newclips)
        {
            int index = overrides.FindIndex(c => c.Key.name.Equals(newclip.name));
            if(index>=0)
            {
                AnimationClip oclip = overrides[index].Key;
                newclip.events = oclip.events;
                overrides[index] = new KeyValuePair<AnimationClip, AnimationClip>(oclip, newclip);
            }
        }
        // foreach (var clip in clips)
        // {
        //     AnimationClip newClip = newclips.Find(c=>c.name.Equals(clip.name));
        //     if(newClip!=null)
        //     {
        //         newClip.events = clip.events;
        //     }
            
        // }

        overrideController.ApplyOverrides(overrides);
    }

    public void AnimatorLayer()
    {
        animator.GetLayerWeight(layerIndex);
        
        // Additive
        // 设置动画层的权重，例如人物表情变化，它不会影响其他层的动画，直接叠加给所有动画。还需要在层级手动设置混合方式为Additive , 与Override则是覆盖。
        animator.SetLayerWeight(layerIndex,1); 

        // Sync 当前层级与某个Source Layer同步的。Timing 勾选上和weight决定了同步时长。
        // 例如Injured 和 Base 中的动画状态分布一样。

        // 越下方的layer优先级高。

        // Additive 必须低一层级有动画。

        // 
        animationEvent.time = 0.6f;
        animationEvent.functionName = "DoSomething";
        animationEvent.stringParameter = "Hello";

    }

    public void AnimatorStateInfo()
    {
        animator.GetFloat("");
        animator.GetBool("");
        animator.GetInteger("");

        animator.SetFloat("",1.0f);
        animator.SetBool("",false);
        animator.SetInteger("",1);
        animator.SetTrigger("");

        layerIndex =  animator.GetLayerIndex("Base Layer");
        animatorClipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex).ToList().Find(c=>c.clip.name=="Idle");
        animatorClipInfo.clip.AddEvent(animationEvent);
        animator.GetCurrentAnimatorClipInfoCount(layerIndex);

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex); //对应的就是动画片段在动画机中的信息。
        currentStateInfo.IsTag("CantMove");

        // 下面这两个参数需要手动去Animator中设置，通过Animator.SetFloat()或者其他set方法去调整
        // currentStateInfo.speedMultiplier //代码不能调整这个参数 ， 通过定义float参数，手动控制动画播放速度 , 无限制，可负可正。
        // currentStateInfo.motionTime //代码没有这个参数 ， 通过定义float参数，手动控制动画播放进度， 0-1之间。
        // currentStateInfo.mirror //代码没有这个参数, 通过定义bool参数，手动控制动画的镜像播放。人物动画有效，将人物动画从上到下一分为二，左边播放的数据变成右边，或者从右边播放的数据变成左边。
        // currentStateInfo.cycleOffset //代码没有这个参数，通过定义float参数，在通过代码手动控制动画的循环偏移，0-1之间。也就是控制一个动画起始帧，不会切割动画，仍然会播放所有动画帧。
        // currentStateInfo.FootIK 略微调整脚步位置，改善脚步动画的不自然，不一定满足高质量动画需求，毕竟计算由偏差。
        
        // currentStateInfo.WriteDefaults 代码没有这个参数，它会自动写入默认属性参数(即OnEnable时，他记录动画中transform的各种属性)，
        // 也就是当播放一个动画片段时，该动画会先检测默认属性值，有就强制将实际的GameObject变回默认属性值，他会先执行默认属性的值，再播放动画，这就有可能导致不想要效果。

        int tagHash = currentStateInfo.tagHash;
        tagHash.Equals(Animator.StringToHash("CantMove"));


    }
    private Rigidbody rig;
    private CharacterController controller;
    //使用该脚本回调 ApplyRootMotion 会自动开启，并应用该脚本方法里的内容。这就相当于和applyRootMotion = true; 效果一样。
    // 为了让动画和实际速度贴合才引入了RootMotion，
    void OnAnimatorMove()
    {
        transform.position += animator.deltaPosition;
        transform.rotation *= animator.deltaRotation;


        //UpdateMode 改为 Animated Physics. 通过下面两种方式让动画和实际速度一致。
        rig.velocity = animator.velocity; 
        controller.Move(animator.velocity * Time.deltaTime);
    }
    public void RootMotion()
    {
        //动画文件会直接修改每一帧游戏对象的坐标值和角度值，RootMotion会通过相对位移和转角来移动对象。
        animator.applyRootMotion = true; // 开启根动作 , 会记录

        // RootNode ， root motion 计算的起点，一般是动画文件中设置的root节点，如果没有设置，则默认是transform的根节点。

        // Model Importer 中 Root Transform Rotation,Root Transform Position,都是表示启用RootMotion时，
        // 是否让模型更着动画动，旁边Loop Match 绿色表示，可以限制RootMotion不要应用，因为他和原动画基本不影响。

        // Humanoid Root Motion : 根据Center of Mass, 和Avatar自动生成的Hip根节点很靠近，根据该点在地面上的投影作为RootMotion的起点

        animator.speed /= animator.humanScale; // 让应用了RootMotion的动画播放速度保持绝对的一致， humanScale 是相对于原骨骼的缩放比例。
    }

    public void Transitions()
    {
        currentTransitionInfo = animator.GetAnimatorTransitionInfo(layerIndex);
        
        // currentTransitionInfo.nameHash;
        currentTransitionInfo.IsUserName("");
        currentTransitionInfo.IsName("");
        // currentTransitionInfo.userNameHash
        // currentTransitionInfo.anyState
        

        // 一个状态到另一状态可以有多个Transitions，点击Transitions线条可以看见。一般多个条件谁先触发，谁先执行 ,某个转换Solo = true，只执行它。多个转换勾选了Solo，那个优先满足，执行那个转换，不会考虑其他转换。

        // 一个状态到多种状态有多个Transitions，状态的WriteDefaults下面可看见。勾选了Solo转换不考虑其他转换，在勾选了Solo的转换中，那个条件先满足就先执行那个转换,同时转换，执行最上面的。

        // 勾选了Mute的转换永远不会被执行。

        // 同个转换中多个条件是 逻辑与 关系被触发 ，上面的是逻辑或

        // has Exit Time ，如果为true， 动画状态转换时间，该条件自动执行。不用设置Condition也可转换。
        // ExitTime 动画退出时机选择，> 0 , 1 表示播放完当前状态进行转换，0.5表示播放一半进行转换， 1.5 表示播放完当前动画，再播放一半进行转换，依次类推。
        // FixedDuration  = true ，则 Transition Duration 参数则表示秒，表示完成转换的时间需要多少秒 (不是两个动画所有时间，仅仅是两个动画正在装欢的间隔)。
        // FixedDuration  = false , 则 Transition Duration 参数则表示百分比，表示完成转换的时间占当前动画总时间的百分比。
        // Transition offset  动画切换的偏移，0-1之间，表示下一个动画从什么位置开始播放，0表示从头开始，0.5表示从中间开始，1表示从尾部开始。

        // Interruption Source 表示当前动画状态可被什么状态打断，None则表示不能被打断。
        // 再多层动画中，底层动画从idle变成了run，上层动画比如人物表情需要从normal，hurry到nervous，需要立刻转到nervous，此时可设置Interruption Source
        // Interruption Source 有多个枚举值，如下：

        // Current State : 表示从当前出发的状态，可以被某些同样 从当前出发的状态 打断。
        // 如果Ordered Interruption = true，则能打断当前转换的只有在当前转换之上优先级的转换。
        // 如果Ordered Interruption = false，则没有优先级，谁都能打断。

        // Next State : 表示从当前出发前往的下一个状态的转换，可以由全部从下一个状态的转换打断。下一个状态的转换是由优先级决定的。

        // Current State Then Next State : 表示从当前出发的状态，可以被当前出发的其他状态和转换的下一个状态的转换都可以打断。从当前出发的状态优先级更高。

        // Next State Then Current State : 表示从当前出发的状态，可以被当前出发的其他状态和转换的下一个状态的转换都可以打断。从下一个状态出发优先级更高。
        // Ordered Interruption ： 就是当前状态WriteDefaults下面的默认排序的优先级。



    } 

    // 一般动画制作无论动捕还是什么的，都是FK(正向动力学)，从骨骼根节点计算，到末梢骨骼节点，依次计算，旋转，位移缩放决定骨骼最终位置。 

    // 有些时候，需要从末梢节点推断，根节点位置，就需要IK，例如人物攀爬墙壁，需要固定手和脚的位置，此时没有动画片段知道父节点状态，所以需要反向计算。IK

    // 如果不设置， IK Pos 会由Avatar推算出来的。 
    void OnAnimatorIK(int layerIndex)
    {
        // IK 位置 Unity根据Avatar会自动生成IK。
        // AvatarIKHint 是关节
        // AvatarIKGoal 是四肢末梢
        //不一定满足高质量动画需求，毕竟 IK计算有偏差。
        animator.SetIKPosition(AvatarIKGoal.RightFoot, Vector3.zero); // 这里设置右脚的IK位置(使用Unity默认生成的就好)，可以使用更复杂的位置计算方法，根据当前人物位置，计算地面IK位置。
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1); // 0~1，1表示完全靠向IK位置。
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1); // 设置唯一
        animator.SetIKHintPosition(AvatarIKHint.LeftKnee,Vector3.zero); // 这里是关节

        //设置头部IK
        animator.SetLookAtPosition(Vector3.forward);
        animator.SetLookAtWeight(1);

    }
}
