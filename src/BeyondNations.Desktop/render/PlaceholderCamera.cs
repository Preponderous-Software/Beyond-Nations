using System;
using System.Numerics;

namespace beyondnations.desktop.render {

    /**
    * The least camera that lets the renderer be seen working.
    *
    * #219 owns the real one -- first and third person, mouse look, the render
    * distance the player can already change from the config commands. Nothing
    * here anticipates any of that. It exists because a renderer cannot be run,
    * let alone verified, without some view and projection matrix, and because
    * inventing a temporary one inside the renderer would have buried the seam.
    *
    * Replacing this means deleting the file and handing PrimitiveRenderer.render
    * two different matrices; the renderer neither knows nor cares where they
    * come from.
    */
    public class PlaceholderCamera {
        private float fieldOfViewDegrees = 60f;
        private float nearPlane = 0.1f;
        private float farPlane = 2000f;
        private float aspectRatio = 16f / 9f;

        /**
        * A fixed offset from whatever is being followed: back along +Z and
        * above, looking down at it.
        */
        private Vector3 offset = new Vector3(0f, 14f, 24f);

        private Vector3 target = Vector3.Zero;

        public void setAspectRatio(int width, int height) {
            if (width <= 0 || height <= 0) {
                return;
            }
            aspectRatio = (float) width / height;
        }

        public void follow(Vector3 target) {
            this.target = target;
        }

        public Vector3 getEyePosition() {
            return target + offset;
        }

        public Matrix4x4 getViewMatrix() {
            return Matrix4x4.CreateLookAt(getEyePosition(), target, Vector3.UnitY);
        }

        public Matrix4x4 getProjectionMatrix() {
            return Matrix4x4.CreatePerspectiveFieldOfView(
                fieldOfViewDegrees * (float) Math.PI / 180f,
                aspectRatio,
                nearPlane,
                farPlane);
        }
    }
}
