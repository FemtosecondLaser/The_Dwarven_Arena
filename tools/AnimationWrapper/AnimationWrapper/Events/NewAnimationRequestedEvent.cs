﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationWrapper
{
    public class NewAnimationRequestedEvent : PubSubEvent<AnimationSet>
    {
    }
}