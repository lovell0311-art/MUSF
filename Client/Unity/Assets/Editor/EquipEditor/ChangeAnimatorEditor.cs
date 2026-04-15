using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace ETEditor {

    public class ChangeAnimator : Editor
    {

        [MenuItem("Assets/改变动画")]
        public static void DoingChange()
        {
            string[] args = Selection.assetGUIDs;
            for (int i = 0; i < args.Length; i++)
            {
                string assetpath = AssetDatabase.GUIDToAssetPath(args[i]);
                GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
                Animator animator = obj.GetComponent<Animator>();
                AnimatorController animatorController = (AnimatorController)animator.runtimeAnimatorController;
                //参数
                AnimatorControllerParameter[] animatorControllerParameters = animatorController.parameters;
                for (int j = 0; j < animatorControllerParameters.Length; j++)
                {
                    if (animatorControllerParameters[j].name == "Dead")
                    {
                        animatorControllerParameters[j].type = AnimatorControllerParameterType.Bool;
                    }
                }
                animatorController.parameters = animatorControllerParameters;

                //状态机
                AnimatorControllerLayer[] animatorControllerLayer = animatorController.layers;
                for (int j = 0; j < animatorControllerLayer.Length; j++)
                {
                    if (animatorControllerLayer[j].name == "Base Layer")
                    {
                        AnimatorStateMachine animatorStateMachine = animatorControllerLayer[j].stateMachine;
                        AnimatorStateTransition[] animatorStateTransition = animatorStateMachine.anyStateTransitions;
                        for (int k = 0; k < animatorStateTransition.Length; k++)
                        {
                            string stateName = animatorStateTransition[k].destinationState.name;
                            //Dead状态
                            AnimatorCondition[] animatorCondition = animatorStateTransition[k].conditions;
                            for (int l = 0; l < animatorCondition.Length; l++)
                            {
                                if (stateName == "Dead" && animatorCondition[l].parameter == "Dead")
                                {
                                    animatorCondition[l].mode = AnimatorConditionMode.If;
                                }
                            }
                            animatorStateTransition[k].conditions = animatorCondition;
                            //Attack状态
                            if (stateName == "Attack" && IsHaveTheState(animatorCondition))
                            {
                                animatorStateTransition[k].AddCondition(AnimatorConditionMode.IfNot, 0f, "Dead");
                            }
                            //Hit状态
                            if (stateName == "Hit" && IsHaveTheState(animatorCondition))
                            {
                                animatorStateTransition[k].AddCondition(AnimatorConditionMode.IfNot, 0f, "Dead");
                            }
                        }
                    }
                }
            }
        }
        public static bool IsHaveTheState(AnimatorCondition[] animatorCondition, string name = "Dead")
        {
            for (int i = 0; i < animatorCondition.Length; i++)
            {
                if (animatorCondition[i].parameter == name)
                    return false;
            }
            return true;
        }

        [MenuItem("Assets/修改Dead")]
        public static void ChangeDead()
        {
            string[] args = Selection.assetGUIDs;
            for (int i = 0; i < args.Length; i++)
            {
                string assetpath = AssetDatabase.GUIDToAssetPath(args[i]);
                GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
                Animator animator = obj.GetComponent<Animator>();
                AnimatorController animatorController = (AnimatorController)animator.runtimeAnimatorController;
                AnimatorControllerLayer[] animatorControllerLayer = animatorController.layers;
                for (int j = 0; j < animatorControllerLayer.Length; j++)
                {
                    if (animatorControllerLayer[j].name == "Base Layer")
                    {
                        AnimatorStateMachine animatorStateMachine = animatorControllerLayer[j].stateMachine;
                        AnimatorStateTransition[] animatorStateTransition = animatorStateMachine.anyStateTransitions;
                        for (int k = 0; k < animatorStateTransition.Length; k++)
                        {
                            string stateName = animatorStateTransition[k].destinationState.name;
                            if (stateName == "Dead") {
                                animatorStateTransition[k].canTransitionToSelf = false;
                            }
                        }
                    }
                }
            }
        }
    }
}