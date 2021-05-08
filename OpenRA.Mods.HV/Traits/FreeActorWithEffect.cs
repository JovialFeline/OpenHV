#region Copyright & License Information
/*
 * Copyright 2019-2021 The OpenHV Developers (see CREDITS)
 * This file is part of OpenHV, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using OpenRA.Mods.Common;
using OpenRA.Mods.Common.Effects;
using OpenRA.Mods.Common.Traits;
using OpenRA.Primitives;
using OpenRA.Traits;

namespace OpenRA.Mods.HV.Traits
{
	[Desc("Player receives a unit for free once the building is placed with a sprite effect.",
		"If you want more than one unit to be spawned, copy this section and assign IDs like FreeActorWithDelay@2, ...")]
	public class FreeActorWithEffectInfo : FreeActorInfo
	{
		public readonly string Image = null;

		[SequenceReference(nameof(Image))]
		[Desc("Sequence to use for overlay animation.")]
		public readonly string Sequence = null;

		[PaletteReference]
		[Desc("Custom palette name")]
		public readonly string Palette = null;

		public override object Create(ActorInitializer init) { return new FreeActorWithEffect(init, this); }
	}

	public class FreeActorWithEffect : FreeActor
	{
		readonly FreeActorWithEffectInfo info;

		public FreeActorWithEffect(ActorInitializer init, FreeActorWithEffectInfo info)
			: base(init, info)
		{
			this.info = info;
		}

		protected override void TraitEnabled(Actor self)
		{
			if (!allowSpawn)
				return;

			allowSpawn = info.AllowRespawn;

			var location = self.Location + Info.SpawnOffset;
			var position = self.World.Map.CenterOfCell(location);
			self.World.AddFrameEndTask(w => w.Add(new SpriteEffect(position, w, info.Image, info.Sequence, info.Palette)));

			self.World.AddFrameEndTask(w =>
			{
				w.CreateActor(Info.Actor, new TypeDictionary
				{
					new ParentActorInit(self),
					new LocationInit(location),
					new OwnerInit(self.Owner),
					new FacingInit(Info.Facing),
				});
			});
		}
	}
}
