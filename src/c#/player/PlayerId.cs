using System;

namespace osg {

    public class PlayerId {
        private Guid guid;

        public PlayerId() {
            guid = Guid.NewGuid();
        }

        public override string ToString() {
            return guid.ToString();
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            PlayerId other = (PlayerId) obj;
            return guid.Equals(other.guid);
        }

        public override int GetHashCode() {
            return guid.GetHashCode();
        }
    }
}