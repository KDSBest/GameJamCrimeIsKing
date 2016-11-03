#if UNITY_EDITOR

using UnityEditor;

using UnityEngine;

namespace Ofm.Framework.Platform
{
    /// <summary>
    /// Helps the developer with handling anchor points in the editor. Do not instance this class.
    /// </summary>
    public class UGuiAnchorToolsEditor : MonoBehaviour
    {
        /// <summary>Move the anchors to the corners of the RectTransform.</summary>
        [MenuItem("uGUI/Anchors to Corners _F5")]
        private static void AnchorsToCorners()
        {
            if (Selection.transforms == null)
                return;

            Undo.RecordObjects(Selection.transforms, "Set Anchors to Corners");
            foreach (Transform transform in Selection.transforms)
            {
                var t = transform as RectTransform;
                var pt = Selection.activeTransform.parent as RectTransform;

                if (t == null || pt == null)
                {
                    return;
                }

                var newAnchorsMin = new Vector2(t.anchorMin.x + (t.offsetMin.x / pt.rect.width), t.anchorMin.y + (t.offsetMin.y / pt.rect.height));
                var newAnchorsMax = new Vector2(t.anchorMax.x + (t.offsetMax.x / pt.rect.width), t.anchorMax.y + (t.offsetMax.y / pt.rect.height));

                t.anchorMin = newAnchorsMin;
                t.anchorMax = newAnchorsMax;
                t.offsetMin = t.offsetMax = new Vector2(0, 0);
                EditorUtility.SetDirty(transform);
            }
        }

        /// <summary>Moves the corners to the anchors points of the RectTransform.</summary>
        [MenuItem("uGUI/Corners to Anchors _F6")]
        private static void CornersToAnchors()
        {
            if (Selection.transforms == null)
                return;
            
            Undo.RecordObjects(Selection.transforms, "Set Corners to Anchors");
            foreach (Transform transform in Selection.transforms)
            {
                var t = transform as RectTransform;

                if (t == null)
                {
                    return;
                }

                t.offsetMin = t.offsetMax = new Vector2(0, 0);
                EditorUtility.SetDirty(transform);
            }
        }

        /// <summary>Mirrors the RectTransform horizontally around the anchorpoints.</summary>
        [MenuItem("uGUI/Mirror Horizontally Around Anchors %#;")]
        private static void MirrorHorizontallyAnchors()
        {
            if (Selection.transforms == null)
                return;
            
            Undo.RecordObjects(Selection.transforms, "Mirror Horizontally Around Anchors");
            MirrorHorizontally(false);
        }

        /// <summary>Mirrors the RectTransform horizontally around the parent centerpoint.</summary>
        [MenuItem("uGUI/Mirror Horizontally Around Parent Center %#:")]
        private static void MirrorHorizontallyParent()
        {
            if (Selection.transforms == null)
                return;
            
            Undo.RecordObjects(Selection.transforms, "Mirror Horizontally Around Parent Center");
            MirrorHorizontally(true);
        }

        /// <summary>Mirrors the RectTransform vertically around anchorpoints.</summary>
        [MenuItem("uGUI/Mirror Vertically Around Anchors %#'")]
        private static void MirrorVerticallyAnchors()
        {
            if (Selection.transforms == null)
                return;
            
            Undo.RecordObjects(Selection.transforms, "Mirror Vertically Around Anchors");
            MirrorVertically(false);
        }

        /// <summary>Mirrors the RectTransform vertically around the parent centerpoint.</summary>
        [MenuItem("uGUI/Mirror Vertically Around Parent Center %#\"")]
        private static void MirrorVerticallyParent()
        {
            if (Selection.transforms == null)
                return;
            
            Undo.RecordObjects(Selection.transforms, "Mirror Vertically Around Parent Center");
            MirrorVertically(true);
        }

        /// <summary>Mirrors the RectTransform or its anchors horizontally.</summary>
        /// <param name="mirrorAnchors">if set to <c>true</c> [mirror anchors].</param>
        private static void MirrorHorizontally(bool mirrorAnchors)
        {
            foreach (Transform transform in Selection.transforms)
            {
                var t = transform as RectTransform;
                var pt = Selection.activeTransform.parent as RectTransform;

                if (t == null || pt == null)
                {
                    return;
                }

                if (mirrorAnchors)
                {
                    Vector2 oldAnchorMin = t.anchorMin;
                    t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);
                    t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);
                }

                Vector2 oldOffsetMin = t.offsetMin;
                t.offsetMin = new Vector2(-t.offsetMax.x, t.offsetMin.y);
                t.offsetMax = new Vector2(-oldOffsetMin.x, t.offsetMax.y);

                t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
                EditorUtility.SetDirty(transform);
            }
        }

        /// <summary>Mirrors the RectTransform or its anchors vertically.</summary>
        /// <param name="mirrorAnchors">if set to <c>true</c> [mirror anchors].</param>
        private static void MirrorVertically(bool mirrorAnchors)
        {
            foreach (Transform transform in Selection.transforms)
            {
                var t = transform as RectTransform;
                var pt = Selection.activeTransform.parent as RectTransform;

                if (t == null || pt == null)
                {
                    return;
                }

                if (mirrorAnchors)
                {
                    Vector2 oldAnchorMin = t.anchorMin;
                    t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);
                    t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);
                }

                Vector2 oldOffsetMin = t.offsetMin;
                t.offsetMin = new Vector2(t.offsetMin.x, -t.offsetMax.y);
                t.offsetMax = new Vector2(t.offsetMax.x, -oldOffsetMin.y);

                t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);

                EditorUtility.SetDirty(transform);
            }
        }
    }
}
#endif
