# LogReader
by Nick Kooy (Rat#3640)
A thing about reading logs and necrodancer and being real grimy about it.


Here's how to use it!

Step 0a) Run as Administrator
The program needs to read the logs in the necrodancer install directory. That's usually some protected ish right there. Admin mode gets around that jazz.
Alternatively you could give READ permissions to that directory. Probably for you and maybe for all users. idk. If you try this and it works. Let me know pls k thx.

Step 0b) Enable the in game logs
In the game press F7. That toggles the logs off and on. The game doesn't say hey now! there's logs going! after you do that. But there's a couple ways to tell if it's running.
1) First check if there's stuff coming into the log reader. It's initially set-up to read only items. So like reset a bunch. And if nothing shows up. Then press F7. Then reset some more. If nothing shows up, then there's another problem happening.
2) You can also take a sneaky peek at your necrodancer logs folder. If the latest log is larger than 1KB. Then blamo it's writing the logs.

# Set-up

Step 1) Set the Necrodancer install directory
Click the little folder icon to navigate there or just edit the text to change the directory.
My game installed to:
C:\Program Files (x86)\Steam\steamapps\common\Crypt of the NecroDancer

But maybe yours is on a different drive or something wacky? Only you would know! Set that ish.

Step 2) Set your Reset key bindings
Open up the reset memes window. 
Click Settings.

-Change the reset key to whatever character you use. I use 'r'. It's for reset. Clearly.
-Change the pause key to whatever character you use to pause. 
-Probably don't change the window name. Unless the game window is called something else in a different language or something. idk.
If that's not set properly it won't send the keystrokes to the right window. got it? good.

Click okay to save those settings.

For more about key characters and special keys go here:
https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys(v=vs.110).aspx#Anchor_3


# How to use

Okay you're probably here to use the reset memery. So i'll start with that one.

So this thing works by reading the logs for items. If the item name matches one of your Expressions. The game will pause.
If it doesn't any of your expressions, then the resets won't stop. They won't stop coming. and they won't stop coming and they won't stop coming and they won't stop coming and they won't stop coming and they won't stop coming and they won't stop coming until they find what you want.

Expressions?! what the heck are those? Okay that's just a word for a logical set of instructions.
By default you only start with one expression:
shovel_blood & familiar_shopkeeper

That means it's looking for a the blood shovel and the shopkeeper familiar. Pretty simple yeah?

Double click beneath the current expression to add another.
That should open up a text box as well for you to edit.

I don't really feel like explaining how to do the logic part. So here's the symbols you can use:
( )	Parens for grouping stuff together
|	Pipe for logical OR
&	Ampersand for logical AND

Every other non-space character will be treated as part of an item name that you are looking for. 
Spaces are ignored and removed. If there are spaces between the words, then those words will be smashed together. 'blood shovel' will become 'bloodshovel' which isn't an item name. So it will never be true.

If you want a list of the item names, check out the necrodancer.xml file. I'll also add the current item list to the end of this README.

Here's some expressions that I've been using:
(shovel_blood | shovel_strength) & familiar_shopkeeper & weapon_golden & (!golden_harp) & (!golden_staff)
(shovel_blood | shovel_strength) & weapon_golden & (spell_heal & (ring_mana | ring_becoming))
(spell_bomb | spell_heal) & charm_bomb & ring_manashovel_blood

To get it going click start. It'll say something like 'Load a level in Necrodancer to begin.'
Once it reads a level loading it'll starting doing it's magickery. It won't stop until you click stop or it finds the stuff.


Okay I'm sick of typing. I'll add more about the other options, formats, and alerts later. Hopefully it's straight forward enough that you can figure it out.
If stuff breaks pls let me know and I'll push out changes as needed.

<items><items>
armor_chainmail

armor_heavyglass

armor_heavyplate

armor_leather

armor_platemail

armor_platemail_dorian

armor_gi

armor_glass

armor_obsidian

armor_quartz

coins_x15

coins_x2

cursed_potion

feet_ballet_shoes

feet_boots_speed

feet_boots_winged

feet_boots_explorers

feet_boots_lead

feet_boots_leaping

feet_boots_lunging

feet_boots_pain

feet_greaves

feet_boots_strength

feet_glass_slippers

food_1

food_2

food_3

food_4

food_carrot

food_cookies

food_magic_1

food_magic_2

food_magic_3

food_magic_4

food_magic_carrot

food_magic_cookies

holy_water

lord_crown

bomb

bomb_3

bomb_grenade

war_drum

blood_drum

double_heart_transplant

heart_transplant

head_crown_of_thorns

head_crown_of_greed

head_crown_of_teleportation

head_circlet_telepathy

head_miners_cap

head_monocle

head_ninja_mask

head_helm

head_glass_jaw

head_blast_helm

head_spiked_ears

head_sunglasses

head_sonar

hud_backpack

holster

bag_holding

misc_compass

misc_coupon

misc_golden_key

misc_golden_key2

misc_golden_key3

misc_glass_key

misc_heart_container

misc_heart_container2

misc_heart_container_cursed

misc_heart_container_cursed2

misc_heart_container_empty

misc_heart_container_empty2

misc_key

misc_magnet

misc_map

misc_monkey_paw

misc_potion

charm_bomb

charm_frost

charm_gluttony

charm_grenade

charm_luck

charm_nazar

charm_protection

charm_risk

charm_strength

perm_heart2

perm_heart3

perm_heart4

perm_heart5

perm_heart6

ring_courage

ring_war

ring_peace

ring_mana

ring_shadows

ring_might

ring_charisma

ring_luck

ring_gold

ring_phasing

ring_piercing

ring_regeneration

ring_protection

ring_shielding

ring_becoming

ring_wonder

ring_pain

ring_frost

scroll_earthquake

scroll_fear

scroll_fireball

scroll_freeze_enemies

scroll_gigantism

scroll_riches

scroll_shield

scroll_enchant_weapon

scroll_need

scroll_pulse

scroll_transmute

shovel_crystal

shovel_battle

shovel_titanium

shovel_blood

shovel_obsidian

shovel_glass

shovel_shard

shovel_basic

shovel_courage

shovel_strength

pickaxe

spell_earth

spell_fireball

spell_pulse

spell_freeze_enemies

spell_heal

spell_bomb

spell_shield

spell_transmute

spell_charm

spell_transform

throwing_stars

familiar_dove

familiar_ice_spirit

familiar_shopkeeper

familiar_shield

familiar_rat

tome_earth

tome_fireball

tome_freeze

tome_pulse

tome_shield

tome_transmute

torch_1

torch_2

torch_3

torch_foresight

torch_glass

torch_infernal

torch_obsidian

torch_strength

torch_walls

        <!-- WEAPONS -->
weapon_eli

weapon_fangs

weapon_flower

weapon_golden_lute

weapon_dagger

weapon_dagger_shard

weapon_titanium_dagger

weapon_obsidian_dagger

weapon_golden_dagger

weapon_blood_dagger

weapon_glass_dagger

weapon_dagger_electric

weapon_dagger_jeweled

weapon_dagger_frost

weapon_dagger_phasing

weapon_broadsword

weapon_titanium_broadsword

weapon_obsidian_broadsword

weapon_golden_broadsword

weapon_blood_broadsword

weapon_glass_broadsword

weapon_longsword

weapon_titanium_longsword

weapon_obsidian_longsword

weapon_golden_longsword

weapon_blood_longsword

weapon_glass_longsword

weapon_whip

weapon_titanium_whip

weapon_obsidian_whip

weapon_golden_whip

weapon_blood_whip

weapon_glass_whip

weapon_spear

weapon_titanium_spear

weapon_obsidian_spear

weapon_golden_spear

weapon_blood_spear

weapon_glass_spear

weapon_rapier

weapon_titanium_rapier

weapon_obsidian_rapier

weapon_golden_rapier

weapon_blood_rapier

weapon_glass_rapier

weapon_bow

weapon_titanium_bow

weapon_obsidian_bow

weapon_golden_bow

weapon_blood_bow

weapon_glass_bow

weapon_crossbow

weapon_titanium_crossbow

weapon_obsidian_crossbow

weapon_golden_crossbow

weapon_blood_crossbow

weapon_glass_crossbow

weapon_flail

weapon_titanium_flail

weapon_obsidian_flail

weapon_golden_flail

weapon_blood_flail

weapon_glass_flail

weapon_cat

weapon_titanium_cat

weapon_obsidian_cat

weapon_golden_cat

weapon_blood_cat

weapon_glass_cat

weapon_blunderbuss

weapon_rifle

weapon_axe

weapon_titanium_axe

weapon_obsidian_axe

weapon_golden_axe

weapon_blood_axe

weapon_glass_axe

weapon_harp

weapon_titanium_harp

weapon_obsidian_harp

weapon_golden_harp

weapon_blood_harp

weapon_glass_harp

weapon_warhammer

weapon_titanium_warhammer

weapon_obsidian_warhammer

weapon_golden_warhammer

weapon_blood_warhammer

weapon_glass_warhammer

weapon_staff

weapon_titanium_staff

weapon_obsidian_staff

weapon_golden_staff

weapon_blood_staff

weapon_glass_staff

weapon_cutlass

weapon_titanium_cutlass

weapon_obsidian_cutlass

weapon_golden_cutlass

weapon_blood_cutlass

weapon_glass_cutlass

	</items>
