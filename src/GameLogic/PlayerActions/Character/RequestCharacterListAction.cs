﻿// <copyright file="RequestCharacterListAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    /// <summary>
    /// Action to request the character list.
    /// </summary>
    public class RequestCharacterListAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestCharacterListAction"/> class.
        /// </summary>
        public RequestCharacterListAction()
        {
        }

        /// <summary>
        /// Requests the character list and advances the player state to <see cref="PlayerState.CharacterSelection"/>.
        /// </summary>
        /// <param name="player">The player who requests the character list.</param>
        public void RequestCharacterList(Player player)
        {
            if (player.PlayerState.TryAdvanceTo(PlayerState.CharacterSelection))
            {
                player.PlayerView.ShowCharacterList();
            }
        }
    }
}
