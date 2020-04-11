﻿// <copyright file="ResetCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character.Reset
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Login;

    /// <summary>
    /// Action to reset a character.
    /// </summary>
    public class ResetCharacterAction
    {
        /// <summary>
        /// Is reset system enabled.
        /// </summary>
        public static readonly bool IsEnabled = true;
        private readonly Player player;
        private readonly LogoutAction logoutAction = new LogoutAction();

        /// <summary>
        /// Reset System Configuration.
        /// </summary>
        private readonly int resetLimit = -1;
        private readonly int requiredLevel = 1;
        private readonly int requiredZen = 1;
        private readonly bool multiplyZenByResetCount = true;
        private readonly bool resetStats = true;
        private readonly bool multiplyPointsByResetCount = true;
        private readonly int pointsPerReset = 1500;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetCharacterAction"/> class.
        /// </summary>
        /// <param name="player">Player to reset.</param>
        public ResetCharacterAction(Player player)
        {
            this.player = player;
        }

        /// <summary>
        /// Reset specific character.
        /// </summary>
        public void ResetCharacter()
        {
            this.CheckEnabled();
            this.CheckNpcWindow();
            this.CheckLevel();
            this.CheckResetLimit();
            this.ConsumeZen();
            this.AddReset();
            this.ResetLevel();
            this.UpdateStats();
            this.MoveHome();
            this.Logout();
        }

        private void CheckEnabled()
        {
            if (!IsEnabled)
            {
                throw new ResetCharacterException("[Reset system] reset is not disabled");
            }
        }

        private void CheckNpcWindow()
        {
            if (this.player.OpenedNpc != null && this.player.OpenedNpc.Definition.NpcWindow != NpcWindow.Undefined)
            {
                throw new ResetCharacterException("[Reset System] cannot do reset with npc window opened");
            }
        }

        private void CheckLevel()
        {
            if (this.player.Level < this.requiredLevel)
            {
                throw new ResetCharacterException($"[Reset System] Required level is {this.requiredLevel}");
            }
        }

        private int GetResetCount()
        {
            return (int)this.player.Attributes[Stats.Resets];
        }

        private void CheckResetLimit()
        {
            if (this.resetLimit > 0 && (this.GetResetCount() + 1) > this.resetLimit)
            {
                throw new ResetCharacterException($"[Reset System] Max reset is {this.resetLimit}");
            }
        }

        private void ConsumeZen()
        {
            var requiredZen = this.requiredZen;
            if (this.multiplyZenByResetCount)
            {
                requiredZen *= this.GetResetCount() + 1;
            }

            if (!this.player.TryRemoveMoney(requiredZen))
            {
                throw new ResetCharacterException($"[Reset System] You don't have enough Money, required zen is {requiredZen}");
            }
        }

        private void AddReset()
        {
            this.player.Attributes[Stats.Resets]++;
        }

        private void ResetLevel()
        {
            this.player.Attributes[Stats.Level] = 1;
            this.player.SelectedCharacter.Experience = 0;
        }

        private void UpdateStats()
        {
            var pointsPerReset = this.pointsPerReset;
            if (this.multiplyPointsByResetCount)
            {
                pointsPerReset *= this.GetResetCount();
            }

            if (this.resetStats)
            {
                this.player.SelectedCharacter.CharacterClass.StatAttributes.ForEach(stat => this.player.Attributes[stat.Attribute] = stat.BaseValue);
            }

            this.player.SelectedCharacter.LevelUpPoints = pointsPerReset;
        }

        private void MoveHome()
        {
            var homeMap = this.player.SelectedCharacter.CharacterClass.HomeMap;
            var randomSpawn = homeMap.SafezoneMap.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
            this.player.SelectedCharacter.PositionX = (byte)Rand.NextInt(randomSpawn.X1, randomSpawn.X2);
            this.player.SelectedCharacter.PositionY = (byte)Rand.NextInt(randomSpawn.Y1, randomSpawn.Y2);
            this.player.SelectedCharacter.CurrentMap = randomSpawn.Map;
            this.player.Rotation = randomSpawn.Direction;
        }

        private void Logout()
        {
            this.logoutAction.Logout(this.player, LogoutType.BackToCharacterSelection);
        }
    }
}
