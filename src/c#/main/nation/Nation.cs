using System.Collections.Generic;
using UnityEngine;

namespace osg
{
    public class Nation
    {
        private NationId id;
        private string name;
        private EntityId leaderId;
        private List<EntityId> members = new List<EntityId>();

        // color
        private Color color;

        public Nation(string name, EntityId leaderId)
        {
            id = new NationId();
            this.name = name;
            this.leaderId = leaderId;
            addMember(leaderId);

            // random color
            color = new Color(Random.value, Random.value, Random.value);
        }

        public NationId getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }

        public EntityId getLeaderId()
        {
            return leaderId;
        }

        public void addMember(EntityId memberId)
        {
            members.Add(memberId);
        }

        public void removeMember(EntityId memberId)
        {
            members.Remove(memberId);
        }

        public bool isMember(EntityId memberId)
        {
            return members.Contains(memberId);
        }

        public int getNumberOfMembers()
        {
            return members.Count;
        }

        public Color getColor()
        {
            return color;
        }
    }
}
