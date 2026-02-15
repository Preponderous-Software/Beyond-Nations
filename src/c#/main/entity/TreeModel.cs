using UnityEngine;

namespace beyondnations {
    
    /// <summary>
    /// Creates a procedural 3D tree model.
    /// This replaces the simple primitive shapes (Cylinder + Cube) with a proper mesh-based tree.
    /// Can be easily replaced with an imported 3D model file when available.
    /// </summary>
    public class TreeModel {
        
        public static GameObject CreateTree(Vector3 position, int height) {
            GameObject treeRoot = new GameObject("Tree");
            treeRoot.transform.position = position;
            
            // Create trunk
            GameObject trunk = CreateTrunk(height);
            trunk.transform.parent = treeRoot.transform;
            trunk.transform.localPosition = Vector3.zero;
            
            // Create leaves/canopy
            GameObject leaves = CreateLeaves(height);
            leaves.transform.parent = treeRoot.transform;
            leaves.transform.localPosition = new Vector3(0, height - 1, 0);
            
            return treeRoot;
        }
        
        private static GameObject CreateTrunk(int height) {
            GameObject trunk = new GameObject("Trunk");
            
            MeshFilter meshFilter = trunk.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = trunk.AddComponent<MeshRenderer>();
            
            // Create a cylindrical trunk mesh
            Mesh trunkMesh = CreateCylinderMesh(0.5f, height, 8);
            meshFilter.mesh = trunkMesh;
            
            // Set brown bark material
            Material trunkMaterial = new Material(Shader.Find("Standard"));
            trunkMaterial.color = new Color(0.4f, 0.2f, 0.1f);
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
            
            // Set green foliage material
            Material leavesMaterial = new Material(Shader.Find("Standard"));
            leavesMaterial.color = new Color(0.1f, 0.6f, 0.1f);
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
            
            // Bottom circle
            for (int i = 0; i < segments; i++) {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                vertices[i] = new Vector3(x, 0, z);
                normals[i] = new Vector3(x, 0, z).normalized;
                uvs[i] = new Vector2((float)i / segments, 0);
            }
            
            // Top circle
            for (int i = 0; i < segments; i++) {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * radius * 0.8f; // Slightly tapered
                float z = Mathf.Sin(angle) * radius * 0.8f;
                vertices[segments + i] = new Vector3(x, height, z);
                normals[segments + i] = new Vector3(x, 0, z).normalized;
                uvs[segments + i] = new Vector2((float)i / segments, 1);
            }
            
            // Center points for caps
            vertices[segments * 2] = new Vector3(0, 0, 0); // Bottom center
            vertices[segments * 2 + 1] = new Vector3(0, height, 0); // Top center
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
