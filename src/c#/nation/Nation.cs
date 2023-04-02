namespace osg {

    public class Nation {
        private NationId id;
        private string name;
        private PlayerId leaderId;

        public Nation(string name, PlayerId leaderId) {
            id = new NationId();
            this.name = name;
            this.leaderId = leaderId;
        }

        public NationId getId() {
            return id;
        }

        public string getName() {
            return name;
        }

        public PlayerId getLeaderId() {
            return leaderId;
        }
    }
}