using UnityEditor;

namespace ETEditor
{
    public static class ChooseRoleBuildTools
    {
        [MenuItem("Tools/Diagnostics/Build Code Bundle")]
        public static void BuildCodeBundleMenu()
        {
            BatchBuildCodeBundle.PerformBuild();
        }
    }
}
