namespace beyondnations.desktop.render {

    /**
    * Where the camera sits relative to the player.
    *
    * The Unity build only ever had the third-person view: a camera parented to
    * the player at a fixed local offset. First person (#173) was filed as a
    * feature request against that arrangement and would have meant a second
    * camera rig. Here it is a mode on the one camera, so #173 becomes a key
    * binding and a call to setMode rather than a new subsystem.
    */
    public enum CameraMode {
        ThirdPerson = 0,
        FirstPerson = 1
    }
}
