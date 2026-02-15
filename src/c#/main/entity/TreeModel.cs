using UnityEngine;

namespace beyondnations {
    
    /// <summary>
    /// Creates a procedural 3D tree model.
    /// This replaces the simple primitive shapes (Cylinder + Cube) with a proper mesh-based tree.
    /// Can be easily replaced with an imported 3D model file when available.
    /// Note: The trunk has a slight taper (top is 0.8x the bottom radius) for more realistic appearance.
    /// </summary>
    public class TreeModel {
        
        public static GameObject CreateTree(Vector3 position, int height, string name = "Tree") {
            // Validate height parameter
            if (height <= 0) {
                height = 1;
            }
            
            GameObject treeRoot = new GameObject(name);
            treeRoot.transform.position = position;
            
            // Create trunk - position it to match original primitive cylinder behavior
            // Unity's primitive cylinder is centered, so we offset by height/2 to match
            GameObject trunk = CreateTrunk(height);
            trunk.transform.parent = treeRoot.transform;
            trunk.transform.localPosition = new Vector3(0, height / 2.0f, 0);
            
            // Create leaves/canopy
            GameObject leaves = CreateLeaves(height);
            leaves.transform.parent = treeRoot.transform;
            leaves.transform.localPosition = new Vector3(0, height - 1, 0);
            
            return treeRoot;
        }
        
        /// <summary>
        /// Properly destroys a tree and its associated mesh resources.
        /// Call this instead of Object.Destroy to prevent memory leaks.
        /// </summary>
        public static void DestroyTree(GameObject treeRoot) {
            if (treeRoot == null) return;
            
            // Explicitly destroy the meshes to prevent memory leaks
            Transform trunk = treeRoot.transform.Find("Trunk");
            if (trunk != null) {
                MeshFilter meshFilter = trunk.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.mesh != null) {
                    UnityEngine.Object.Destroy(meshFilter.mesh);
                }
            }
            
            Transform leaves = treeRoot.transform.Find("Leaves");
            if (leaves != null) {
                MeshFilter meshFilter = leaves.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.mesh != null) {
                    UnityEngine.Object.Destroy(meshFilter.mesh);
                }
            }
            
            // Destroy the tree GameObject
            UnityEngine.Object.Destroy(treeRoot);
        }
        
        /// <summary>
        /// Gets a default shader with fallback options.
        /// Tries Standard shader first, then Unlit/Color, and finally uses a built-in fallback.
        /// </summary>
        private static Shader GetDefaultShader() {
            Shader shader = Shader.Find("Standard");
            if (shader == null) {
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null) {
                // Final fallback - use the built-in default diffuse shader
                shader = Shader.Find("Diffuse");
            }
            return shader;
        }
        
        private static GameObject CreateTrunk(int height) {
            GameObject trunk = new GameObject("Trunk");
            
            MeshFilter meshFilter = trunk.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = trunk.AddComponent<MeshRenderer>();
            
            // Create a cylindrical trunk mesh
            Mesh trunkMesh = CreateCylinderMesh(0.5f, height, 8);
            meshFilter.mesh = trunkMesh;
            
            // Set brown bark material (matching original color)
            Material trunkMaterial = new Material(GetDefaultShader());
            trunkMaterial.color = new Color(0.5f, 0.25f, 0);
            meshRenderer.material = trunkMaterial;
            
            return trunk;
        }
        
        private static GameObject CreateLeaves(int height) {
            GameObject leaves = new GameObject("Leaves");
            
            MeshFilter meshFilter = leaves.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = leaves.AddComponent<MeshRenderer>();
            
            // Create a spherical canopy mesh
            Mesh leavesMesh = CreateSphereMesh(2.5f, 10, 10);
            meshFilter.mesh = leavesMesh;
            
            // Set green foliage material (matching original color)
            Material leavesMaterial = new Material(GetDefaultShader());
            leavesMaterial.color = Color.green;
            meshRenderer.material = leavesMaterial;
            
            return leaves;
        }
        
        /// <summary>
        /// Creates a cylindrical mesh for the trunk
        /// </summary>
        private static Mesh CreateCylinderMesh(float radius, float height, int segments) {
            Mesh mesh = new Mesh();
            mesh.name = "CylinderMesh";
            
            int vertexCount = segments * 2 + 2; // Top and bottom circles plus centers
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            
            // Create vertices
            float angleStep = 360f / segments * Mathf.Deg2Rad;
            
            // Bottom circle - centered at -height/2 to match Unity primitive cylinder
            for (int i = 0; i < segments; i++) {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                vertices[i] = new Vector3(x, -height / 2.0f, z);
                normals[i] = new Vector3(x, 0, z).normalized;
                uvs[i] = new Vector2((float)i / segments, 0);
            }
            
            // Top circle - at +height/2 to match Unity primitive cylinder
            for (int i = 0; i < segments; i++) {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * radius * 0.8f; // Slightly tapered for realism
                float z = Mathf.Sin(angle) * radius * 0.8f;
                vertices[segments + i] = new Vector3(x, height / 2.0f, z);
                normals[segments + i] = new Vector3(x, 0, z).normalized;
                uvs[segments + i] = new Vector2((float)i / segments, 1);
            }
            
            // Center points for caps
            vertices[segments * 2] = new Vector3(0, -height / 2.0f, 0); // Bottom center
            vertices[segments * 2 + 1] = new Vector3(0, height / 2.0f, 0); // Top center
            normals[segments * 2] = Vector3.down;
            normals[segments * 2 + 1] = Vector3.up;
            uvs[segments * 2] = new Vector2(0.5f, 0.5f);
            uvs[segments * 2 + 1] = new Vector2(0.5f, 0.5f);
            
            // Create triangles
            int[] triangles = new int[segments * 12];
            int triIndex = 0;
            
            // Side triangles
            for (int i = 0; i < segments; i++) {
                int next = (i + 1) % segments;
                
                // First triangle
                triangles[triIndex++] = i;
                triangles[triIndex++] = segments + i;
                triangles[triIndex++] = next;
                
                // Second triangle
                triangles[triIndex++] = next;
                triangles[triIndex++] = segments + i;
                triangles[triIndex++] = segments + next;
            }
            
            // Bottom cap
            for (int i = 0; i < segments; i++) {
                int next = (i + 1) % segments;
                triangles[triIndex++] = segments * 2;
                triangles[triIndex++] = next;
                triangles[triIndex++] = i;
            }
            
            // Top cap
            for (int i = 0; i < segments; i++) {
                int next = (i + 1) % segments;
                triangles[triIndex++] = segments * 2 + 1;
                triangles[triIndex++] = segments + i;
                triangles[triIndex++] = segments + next;
            }
            
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            
            mesh.RecalculateBounds();
            return mesh;
        }
        
        /// <summary>
        /// Creates a spherical mesh for the leaves
        /// </summary>
        private static Mesh CreateSphereMesh(float radius, int latitudeSegments, int longitudeSegments) {
            Mesh mesh = new Mesh();
            mesh.name = "SphereMesh";
            
            int vertexCount = (latitudeSegments + 1) * (longitudeSegments + 1);
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            
            int vertIndex = 0;
            for (int lat = 0; lat <= latitudeSegments; lat++) {
                float theta = lat * Mathf.PI / latitudeSegments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);
                
                for (int lon = 0; lon <= longitudeSegments; lon++) {
                    float phi = lon * 2 * Mathf.PI / longitudeSegments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);
                    
                    Vector3 normal = new Vector3(cosPhi * sinTheta, cosTheta, sinPhi * sinTheta);
                    vertices[vertIndex] = normal * radius;
                    normals[vertIndex] = normal;
                    uvs[vertIndex] = new Vector2((float)lon / longitudeSegments, (float)lat / latitudeSegments);
                    vertIndex++;
                }
            }
            
            int[] triangles = new int[latitudeSegments * longitudeSegments * 6];
            int triIndex = 0;
            
            for (int lat = 0; lat < latitudeSegments; lat++) {
                for (int lon = 0; lon < longitudeSegments; lon++) {
                    int current = lat * (longitudeSegments + 1) + lon;
                    int next = current + longitudeSegments + 1;
                    
                    triangles[triIndex++] = current;
                    triangles[triIndex++] = next;
                    triangles[triIndex++] = current + 1;
                    
                    triangles[triIndex++] = current + 1;
                    triangles[triIndex++] = next;
                    triangles[triIndex++] = next + 1;
                }
            }
            
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
