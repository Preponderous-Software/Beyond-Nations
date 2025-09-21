using UnityEngine;
using System.Collections.Generic;

namespace beyondnations {
    public class NationManagementInfoBox : InfoBox {
        private Player player;
        private NationRepository nationRepository;
        private EntityRepository entityRepository;
        private EventProducer eventProducer;
        private bool isVisible = false;
        private string newNationName = "";
        private EntityId selectedNewLeaderId = null;

        public NationManagementInfoBox(int x, int y, int width, int height, int padding, string title, Player player, NationRepository nationRepository, EntityRepository entityRepository, EventProducer eventProducer) : base(x, y, width, height, padding, title) {
            this.player = player;
            this.nationRepository = nationRepository;
            this.entityRepository = entityRepository;
            this.eventProducer = eventProducer;
        }

        public void setVisible(bool visible) {
            this.isVisible = visible;
        }

        public bool getVisible() {
            return isVisible;
        }

        public override void draw() {
            if (!isVisible) {
                return;
            }

            if (player.getNationId() == null) {
                return;
            }

            Nation nation = nationRepository.getNation(player.getNationId());
            if (nation == null || nation.getLeaderId() != player.getId()) {
                return;
            }

            int boxHeight = 300;
            int currentY = y;

            // Draw main box
            GUI.Box(new Rect(x - padding, currentY - padding, width + (padding * 2), boxHeight), title);
            currentY += 20;

            // Nation name section
            GUI.Label(new Rect(x, currentY, width, height), "Current Name: " + nation.getName());
            currentY += height;

            GUI.Label(new Rect(x, currentY, 80, height), "New Name:");
            newNationName = GUI.TextField(new Rect(x + 85, currentY, width - 170, height), newNationName);
            if (GUI.Button(new Rect(x + width - 80, currentY, 75, height), "Rename")) {
                NationRenameCommand command = new NationRenameCommand(nationRepository, newNationName);
                command.execute(player);
                newNationName = "";
            }
            currentY += height + 10;

            // Transfer ownership section
            GUI.Label(new Rect(x, currentY, width, height), "Transfer Ownership:");
            currentY += height;

            List<EntityId> members = nation.getMembers();
            int memberCount = 0;
            foreach (EntityId memberId in members) {
                if (memberId.Equals(player.getId())) {
                    continue; // Skip current leader
                }

                Entity member = entityRepository.getEntity(memberId);
                if (member == null) {
                    continue;
                }

                bool isSelected = selectedNewLeaderId != null && selectedNewLeaderId.Equals(memberId);
                string buttonText = (isSelected ? "* " : "") + member.getName();
                
                if (GUI.Button(new Rect(x, currentY, width - 80, height), buttonText)) {
                    selectedNewLeaderId = memberId;
                }

                memberCount++;
                currentY += height;
                
                if (memberCount >= 3) { // Limit display to 3 members to keep UI manageable
                    break;
                }
            }

            if (selectedNewLeaderId != null) {
                if (GUI.Button(new Rect(x + width - 75, currentY - (memberCount * height), 70, height), "Transfer")) {
                    NationTransferOwnershipCommand command = new NationTransferOwnershipCommand(nationRepository, entityRepository, selectedNewLeaderId);
                    command.execute(player);
                    selectedNewLeaderId = null;
                    isVisible = false;
                }
            }

            currentY += 10;

            // Disband section
            GUI.Label(new Rect(x, currentY, width, height), "Danger Zone:");
            currentY += height;

            if (GUI.Button(new Rect(x, currentY, width, height), "Disband Nation")) {
                NationDisbandCommand command = new NationDisbandCommand(nationRepository, entityRepository, eventProducer);
                command.execute(player);
                isVisible = false;
            }
            currentY += height + 10;

            // Close button
            if (GUI.Button(new Rect(x + width - 60, y - padding + 5, 55, 20), "Close")) {
                isVisible = false;
            }
        }
    }
}