//we need the Bow add-on for this, so force it to load
%error = ForceRequiredAddOn("Weapon_Bow");

if(%error == $Error::AddOn_Disabled)
{
	//A bit of a hack:
	//  we just forced the Bow to load, but the user had it disabled
	//  so lets make it so they can't select it
	BowItem.uiName = "";
}

if(%error == $Error::AddOn_NotFound)
{
	//we don't have the Bow, so we're screwed
	error("ERROR: Weapon_Throwables - required add-on Weapon_Bow not found");
	return;
}

exec("./hitregion.cs");
exec("./weapon_throwing_knife.cs");

datablock AudioProfile(spearGore1)
{
	filename	= "./sounds/Thrown_Spear_gore_01.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(spearGore2)
{
	filename	= "./sounds/Thrown_Spear_gore_02.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(spearGore3)
{
	filename	= "./sounds/Thrown_Spear_gore_03.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(spearGore4)
{
	filename	= "./sounds/Thrown_Spear_gore_04.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(spearGore5)
{
	filename	= "./sounds/Thrown_Spear_gore_05.wav";
	description	= AudioClosest3d;
	preload		= true;
};