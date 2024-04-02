using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game._Scripts
{
    public class DrawOnPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField]
        private int thickness = 10;
        
        [SerializeField]
        private RawImage drawingSurface;
        
        [SerializeField]
        private RectTransform drawingRectTransform;
        
        [SerializeField]
        private Texture2D initialTexture;

        [SerializeField]
        private GroupRunners groupRunners;

        [SerializeField]
        private Terrain terrain;

        private Texture2D _drawingTexture;

        public List<Vector2> pathPoints = new List<Vector2>();

        private void Start()
        {
            InitializeDrawingSurface();
        }

        private void InitializeDrawingSurface()
        {
            if (!drawingSurface)
            {
                Debug.LogError("Drawing surface (RawImage) not assigned.");
                return;
            }

            // Clone the initial texture to ensure it's editable
            _drawingTexture = new Texture2D(initialTexture.width, initialTexture.height, TextureFormat.RGBA32, false);
            Graphics.CopyTexture(initialTexture, _drawingTexture);
            _drawingTexture.Apply();

            drawingSurface.texture = _drawingTexture;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Draw(eventData.position);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Draw(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ResetToOriginalTexture();
            
            PositionChildrenAlongPath(ConvertTo3DPoints(pathPoints, Camera.main, terrain));
            ResetPath();
        }

        private void Draw(Vector2 screenPosition)
        {
            // Convert the screen position to local position relative to the drawingSurface
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingRectTransform, screenPosition, null,
                out var localPoint)) return;
        
            var rect = drawingRectTransform.rect;
            
            float x = ((localPoint.x - rect.x) / rect.width) * _drawingTexture.width;
            float y = ((localPoint.y - rect.y) / rect.height) * _drawingTexture.height;

            float thicknessLevel = thickness / 2;
            
            // Calculate the start and end points for both x and y based on thickness
            int startX = Mathf.Max(0, (int)(x - thicknessLevel));
            int startY = Mathf.Max(0, (int)(y - thicknessLevel));
            int endX = Mathf.Min(_drawingTexture.width, (int)(x + thicknessLevel));
            int endY = Mathf.Min(_drawingTexture.height, (int)(y + thicknessLevel));

            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    _drawingTexture.SetPixel(i, j, Color.black);
                }
            }


            _drawingTexture.Apply();
            
            pathPoints.Add(screenPosition);
        }

        private void ResetToOriginalTexture()
        {
            Graphics.CopyTexture(initialTexture, _drawingTexture);
            _drawingTexture.Apply();
        }
        
        void PositionChildrenAlongPath(List<Vector3> pathPoints3D)
        {
            int childCount = groupRunners.PlayerRunners.Count;
            if (childCount == 0 || pathPoints3D.Count < 1)
                return;

            // Calculate total path length and segment lengths
            float totalLength = 0f;
            List<float> segmentLengths = new List<float>();
            for (int i = 0; i < pathPoints3D.Count - 1; i++)
            {
                float segmentLength = Vector3.Distance(pathPoints3D[i], pathPoints3D[i + 1]);
                segmentLengths.Add(segmentLength);
                totalLength += segmentLength;
            }

            // Calculate spacing between runners
            float spacing = totalLength / (childCount - 1);

            float distanceTraveled = 0f;
            for (int i = 0; i < childCount; i++)
            {
                if (i == 0)
                {
                    // First runner goes at the start of the path
                    groupRunners.PlayerRunners[i].transform.position = pathPoints3D[0];
                    continue;
                }

                float targetDistance = spacing * i;

                if (segmentLengths.Count == 0) segmentLengths.Add(0);

                while (distanceTraveled + segmentLengths[0] < targetDistance && segmentLengths.Count > 1)
                {
                    distanceTraveled += segmentLengths[0];
                    segmentLengths.RemoveAt(0);
                    pathPoints3D.RemoveAt(0);
                }

                // Calculate position for the child
                float remainingDistance = targetDistance - distanceTraveled;
                Vector3 direction = pathPoints3D.Count > 1
                    ? (pathPoints3D[1] - pathPoints3D[0]).normalized
                    : pathPoints3D[0];
                Vector3 newPosition = pathPoints3D[0] + direction * remainingDistance;
                groupRunners.PlayerRunners[i].transform.position = newPosition;
            }
        }
        
        public void ResetPath()
        {
            pathPoints.Clear();
        }
        
        public List<Vector3> ConvertTo3DPoints(List<Vector2> pathPoints2D, Camera mainCamera, Terrain terrain)
        {
            List<Vector3> pathPoints3D = new List<Vector3>();
            RaycastHit hit;

            foreach (Vector2 point2D in pathPoints2D)
            {
                Ray ray = mainCamera.ScreenPointToRay(point2D);
                
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == terrain.gameObject)
                    {
                        Vector3 hitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z + 50f);
                        pathPoints3D.Add(hitPoint);
                    }
                }
            }
            
            return pathPoints3D;
        }

    }
}