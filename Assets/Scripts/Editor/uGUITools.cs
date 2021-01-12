using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class uGUITools : MonoBehaviour {
    [MenuItem ("uGUI/Anchors to Corners %[")]
    private static void AnchorsToCorners () {

        Transform[] transforms = Selection.GetTransforms (SelectionMode.Deep | SelectionMode.Editable);

        AnchorsToCorners (transforms);
    }

    [MenuItem ("uGUI/Anchors to Corners Parent %[")]
    private static void AnchorsToCornersParent () {
        Transform[] transforms = Selection.GetTransforms (SelectionMode.TopLevel | SelectionMode.Editable);

        AnchorsToCorners (transforms);
    }

    private static void AnchorsToCorners (Transform[] transforms) {
        for (int i = 0; i < transforms.Length; i++) {

            RectTransform t = transforms[i] as RectTransform;
            RectTransform pt = transforms[i].parent as RectTransform;

            if (t == null || pt == null) continue;

            Vector2 newAnchorsMin = new Vector2 (t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2 (t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2 (0, 0);
        }
    }

    [MenuItem ("uGUI/Anchors to BottomLeft %[")]
    static void AnchorsToBottomLeft () {

        Transform[] transforms = Selection.GetTransforms (SelectionMode.Deep | SelectionMode.Editable);

        for (int i = 0; i < transforms.Length; i++) {

            RectTransform t = transforms[i] as RectTransform;
            RectTransform pt = transforms[i].parent as RectTransform;

            if (t == null || pt == null) continue;

            Vector2 newOffsetMin = new Vector2 (t.anchorMin.x * pt.rect.width + t.offsetMin.x,
                t.anchorMin.y * pt.rect.height + t.offsetMin.y);
            Vector2 newOffsetMax = new Vector2 (t.anchorMax.x * pt.rect.width + t.offsetMax.x,
                t.anchorMax.y * pt.rect.height + t.offsetMax.y);

            t.anchorMin = t.anchorMax = new Vector2 (0, 0);
            t.offsetMin = newOffsetMin;
            t.offsetMax = newOffsetMax;
        }
    }

    [MenuItem ("uGUI/Anchors to TopLeft %[")]
    static void AnchorsToTopLeft () {

        Transform[] transforms = Selection.GetTransforms (SelectionMode.Deep | SelectionMode.Editable);

        for (int i = 0; i < transforms.Length; i++) {

            RectTransform t = transforms[i] as RectTransform;
            RectTransform pt = transforms[i].parent as RectTransform;

            if (t == null || pt == null) continue;

            Vector2 newOffsetMin = new Vector2 (t.anchorMin.x * pt.rect.width + t.offsetMin.x,
                t.anchorMin.y * pt.rect.height + t.offsetMin.y - pt.rect.height);
            Vector2 newOffsetMax = new Vector2 (t.anchorMax.x * pt.rect.width + t.offsetMax.x,
                t.anchorMax.y * pt.rect.height + t.offsetMax.y - pt.rect.height);

            t.anchorMin = t.anchorMax = new Vector2 (0, 1);
            t.offsetMin = newOffsetMin;
            t.offsetMax = newOffsetMax;
        }
    }

    [MenuItem ("uGUI/DisableRaycast %[")]
    public static void DisableRaycast () {
        Transform[] transforms = Selection.GetTransforms (SelectionMode.Deep | SelectionMode.Editable);

        for (int i = 0; i < transforms.Length; i++) {
            MaskableGraphic graphic = transforms[i].GetComponent<MaskableGraphic> ();
            if (graphic != null)
                graphic.raycastTarget = false;
        }
    }

    [MenuItem ("uGUI/ClearPrefs %[")]
    public static void ClearPrefs () {
        PlayerPrefs.DeleteAll();
    }
}