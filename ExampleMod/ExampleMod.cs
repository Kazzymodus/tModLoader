using ExampleMod.Content;
using ExampleMod.Content.Items;
using ExampleMod.Content.Items.Consumables;
using ExampleMod.Content.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace ExampleMod
{
	public class ExampleMod : Mod
	{
		public const string AssetPath = "ExampleMod/Assets/";

		public override void AddRecipes() => ExampleRecipes.Load(this);

		public override void Load() {
			// DO NOT LOAD ASSETS ON SEVER!
			if (!Main.dedServ) {
				GameShaders.Armor.BindShader(ModContent.ItemType<ExampleDye>(), new ArmorShaderData(new Ref<Effect>(GetEffect("Assets/Effects/ExampleEffect").Value), "ExampleDyePass"));
				GameShaders.Hair.BindShader(ModContent.ItemType<ExampleHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Green));
			}
		}

		public override void Unload() => ExampleRecipes.Unload();

		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			ExampleModMessageType msgType = (ExampleModMessageType)reader.ReadByte();
			switch (msgType) {
				// This message syncs ExamplePlayer.exampleLifeFruits
				case ExampleModMessageType.ExamplePlayerSyncPlayer:
					byte playernumber = reader.ReadByte();
					ExampleLifeFruitPlayer examplePlayer = Main.player[playernumber].GetModPlayer<ExampleLifeFruitPlayer>();
					examplePlayer.exampleLifeFruits = reader.ReadInt32();
					// SyncPlayer will be called automatically, so there is no need to forward this data to other clients.
					break;
				case ExampleModMessageType.ExampleTeleportToStatue:
					if (Main.npc[reader.ReadByte()].modNPC is ExamplePerson person && person.npc.active) {
						person.StatueTeleport();
					}

					break;
				default:
					Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}

	internal enum ExampleModMessageType : byte
	{
		ExamplePlayerSyncPlayer,
		ExampleTeleportToStatue
	}
}