using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Resources.Script.UI
{
    [ExecuteAlways]
    public class SetLabelName : MonoBehaviour
    {
    public Transform target;

    private TextMeshProUGUI text;
    private Text legecyText;

    string TargetName;

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (Application.isPlaying == false)
            Refresh();
    }

    private void Reset()
    {
        Refresh();
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        if (!target) target = transform.parent;

        if (target == null) return;

        if (text == null && legecyText == null) text = GetComponent<TextMeshProUGUI>();
        if (text == null && legecyText == null) legecyText = GetComponent<Text>();

        if (TargetName != target.name)
        {
            TargetName = target.name;
        }

        if (text && text.text != TargetName)
            text.text = TargetName;

        if (legecyText && legecyText.text != TargetName)
            legecyText.text = TargetName;
    }
    }
}