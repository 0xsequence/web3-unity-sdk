#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
class BuildSequenceHTML
{

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void CreateAssetWhenReady()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += CreateAssetWhenReady;
            return;
        }

        EditorApplication.delayCall += UpdateSequenceHTML;
    }

    public static void UpdateSequenceHTML()
    {
        var sequenceJSPath = Path.Combine(Application.streamingAssetsPath, "sequence/sequence.js");
        var ethersJSPath = Path.Combine(Application.streamingAssetsPath, "sequence/ethers.js");
        var sequenceHTMLTemplatePath = Path.Combine(Application.streamingAssetsPath, "sequence/sequence.html.template");
        string sequenceJS = File.ReadAllText(sequenceJSPath);

        string ethersJS = File.ReadAllText(ethersJSPath);

        string sequenceHTMLTemplate = File.ReadAllText(sequenceHTMLTemplatePath);

        var split = sequenceHTMLTemplate.Split("<!-- %INJECT_SEQUENCE_JS% -->");
            if(split.Length != 2)
        {
            throw new UnityEditor.Build.BuildFailedException("Sequence HTML template is corrupted - doesn't include one <!-- %INJECT_SEQUENCE_JS% --> marker.");

        }
        var combined = split[0] + @"
<!-- ethers.js -->
<script>
" + ethersJS + @"
</script>
<!-- sequence.js -->
<script>
" + sequenceJS + @"
</script>
" + split[1];

        var sequenceHTMLGeneratedPath = Path.Combine(Application.streamingAssetsPath, "sequence/sequence.html");

        File.WriteAllText(sequenceHTMLGeneratedPath, combined);
    }
}
#endif