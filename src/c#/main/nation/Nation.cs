using System.Collections.Generic;
using UnityEngine;

namespace osg {

    public class Nation {
        private NationId id;
        private string name;
        private EntityId leaderId;
        private List<EntityId> members = new List<EntityId>();
        private Dictionary<EntityId, NationRole> roles = new Dictionary<EntityId, NationRole>();
        private Color color;

        public Nation(string name, EntityId leaderId) {
            id = new NationId();
            this.name = name;
            this.leaderId = leaderId;
            members.Add(leaderId);
            roles[leaderId] = NationRole.LEADER;
            color = new Color(Random.value, Random.value, Random.value);
        }

        public NationId getId() {
            return id;
        }

        public string getName() {
            return name;
        }

        public EntityId getLeaderId() {
            return leaderId;
        }

        public void setLeaderId(EntityId leaderId) {
            this.leaderId = leaderId;
        }

        public void addMember(EntityId memberId) {
            members.Add(memberId);
            roles[memberId] = NationRole.CITIZEN;
        }

        public void removeMember(EntityId memberId) {
            members.Remove(memberId);
            roles.Remove(memberId);
        }

        public bool isMember(EntityId memberId) {
            return members.Contains(memberId);
        }

        public int getNumberOfMembers() {
            return members.Count;
        }

        public Color getColor() {
            return color;
        }

        public NationRole getRole(EntityId memberId) {
            return roles[memberId];
        }

        public void setRole(EntityId memberId, NationRole role) {
            roles[memberId] = role;
        }

        public EntityId getRandomMemberId() {
            int randomIndex = Random.Range(0, members.Count);
            return members[randomIndex];
        }

        public EntityId getOldestMemberId() {
            return members[0];
        }
    }
}