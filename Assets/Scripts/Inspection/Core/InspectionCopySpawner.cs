using UnityEngine;
using Edgar.Characters.Core;
using Edgar.UI;

namespace Edgar.Inspection.Core
{
    public class InspectionCopySpawner : MonoBehaviour
    {
        [SerializeField] private Transform _inspectionAnchor;
        [SerializeField] private InspectionOriginalBehavior _originalBehavior = InspectionOriginalBehavior.Hide;

        public GameObject SpawnCopy(InspectableObject item)
        {
            if (item == null || _inspectionAnchor == null) return null;

            GameObject copy;

            if (item.InspectionPrefab != null)
            {
                copy = Instantiate(item.InspectionPrefab, _inspectionAnchor.position, Quaternion.identity, _inspectionAnchor);
            }
            else
            {
                copy = Instantiate(item.gameObject, _inspectionAnchor.position, Quaternion.identity, _inspectionAnchor);

                var billboard = copy.GetComponent<BillboardSprite>();
                if (billboard != null) Destroy(billboard);

                var sr = copy.GetComponent<SpriteRenderer>();
                if (sr != null) ConvertSpriteToQuad(copy, sr);
            }

            foreach (var col in copy.GetComponents<Collider>())
                Destroy(col);

            var itemComp = copy.GetComponent<InspectableObject>();
            if (itemComp != null) Destroy(itemComp);

            var rb = copy.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);

            var outline = copy.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
                Destroy(outline);
            }

            var icon = copy.GetComponent<InteractableIcon>();
            if (icon != null) Destroy(icon);

            var iconPivot = copy.transform.Find("_InteractableIcon");
            if (iconPivot != null) Destroy(iconPivot.gameObject);

            SetLayerRecursive(copy, LayerMask.NameToLayer("InspectionLayer"));
            copy.transform.localScale = item.InspectionScale;
            CenterOnAnchor(copy);

            return copy;
        }

        public InspectionOriginalBehavior OriginalBehavior => _originalBehavior;

        private void CenterOnAnchor(GameObject copy)
        {
            var renderers = copy.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;

            var bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            copy.transform.position += _inspectionAnchor.position - bounds.center;
        }

        private void ConvertSpriteToQuad(GameObject copy, SpriteRenderer sr)
        {
            var sprite = sr.sprite;
            Destroy(sr);

            if (sprite == null) return;

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(quad.GetComponent<MeshCollider>());
            quad.transform.SetParent(copy.transform, false);

            float aspect = sprite.rect.width / sprite.rect.height;
            quad.transform.localScale = new Vector3(aspect, 1f, 1f);

            // Create double-sided material with transparency support
            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.mainTexture = sprite.texture;

            // Make it double-sided
            mat.SetFloat("_Cull", 0); // 0 = Off (double-sided), 1 = Front, 2 = Back

            // If your sprite uses transparency
            mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent

            quad.GetComponent<MeshRenderer>().material = mat;

            SetLayerRecursive(quad, LayerMask.NameToLayer("InspectionLayer"));
        }

        private void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursive(child.gameObject, layer);
        }
    }
}