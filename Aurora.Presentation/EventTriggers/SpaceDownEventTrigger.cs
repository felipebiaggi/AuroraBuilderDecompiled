﻿using System;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Aurora.Presentation.EventTriggers
{
    public class SpaceDownEventTrigger : EventTrigger
    {
        public SpaceDownEventTrigger()
          : base("KeyDown")
        {
        }

        protected override void OnEvent(EventArgs eventArgs)
        {
            if (!(eventArgs is KeyEventArgs keyEventArgs) || keyEventArgs.Key != Key.Space)
                return;
            this.InvokeActions((object)eventArgs);
        }
    }
}