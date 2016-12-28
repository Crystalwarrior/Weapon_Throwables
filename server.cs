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

if($Pref::Server::Ranged::Ammo["knives"] $= "") $Pref::Server::Ranged::Ammo["knives"] = 20;

datablock ParticleData(RangedBloodExplosionParticle)
{
	dragCoefficient      = 0;
	gravityCoefficient   = 5.0;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	lifetimeMS           = 200;
	lifetimeVarianceMS   = 60;
	textureName          = "base/data/particles/chunk";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.71 0.01 0.01 0.9";
	colors[1]     = "0.71 0.01 0.01 0.6";
	sizes[0]      = 0.1;
	sizes[1]      = 0.1;
	useInvAlpha = true;
};
datablock ParticleEmitterData(RangedBloodExplosionEmitter)
{
   ejectionPeriodMS = 15;
   periodVarianceMS = 2;
   ejectionVelocity = 8;
   velocityVariance = 2;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 20;
   phiReferenceVel  = 0;
   phiVariance      = 25;
   overrideAdvance = false;
   particles = "RangedBloodExplosionParticle";
};

datablock ExplosionData(RangedBloodExplosion)
{
	// soundProfile = bullethitSound;
	lifeTimeMS = 150;
	particleEmitter = RangedBloodExplosionEmitter;
	particleDensity = 5;
	particleRadius = 0.1;
	faceViewer     = true;
	explosionScale = "1 1 1";
	shakeCamera = false;
	camShakeFreq = "0.0 1.0 1.0";
	camShakeAmp = "0.0 3.0 2.5";
	camShakeDuration = 0.5;
	camShakeRadius = 0.5;
	lightStartRadius = 0;
	lightEndRadius = 0;
};

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

exec("./hitregion.cs");
exec("./weapon_throwing_knife.cs");

function Player::updateDisplayAmmo(%this, %disable, %forceType)
{
	if(!isObject(%this.client))
		return;
	%image = %this.getMountedImage(0);
	if(%disable || (!isObject(%image) && %forceType $= ""))
	{
		clearBottomPrint(%this.client);
		return;
	}
	%ammoType = %image.item.ammoType;
	if(%forceType !$= "") %ammoType = %forceType;
	%str = %ammoType SPC %this.ammo[%ammoType];
	%delay = 0;
	if(!isObject(%image))
		%delay = 2;
	%this.client.bottomPrint("<font:BrowalliaUPC:30><just:right><color:FFFFFF>" @ %str SPC "\n", %delay, 1);
}

package RPRangedPackage
{
	function Armor::onCollision(%this, %obj, %col, %velocity, %speed)
	{
		if (isObject(%col) && %col.getClassName() $= "Item" && %col.getDataBlock().ammoType !$= "")
		{
			%obj.pickup(%col);
			return;
		}
		Parent::onCollision(%this, %obj, %col, %velocity, %speed);
	}
	function Player::pickUp(%obj, %item)
	{
		%data = %item.getDatablock();
		if(%data.ammoType !$= "")
		{
			if(isEventPending(%item.fadeInSchedule))
				return;
			for(%i=0;%i<%obj.getDatablock().maxTools;%i++)
			{
				if(isObject(%obj.tool[%i]))
				{
					%tool=%obj.tool[%i];
					if(%tool.ammoType $= %data.ammoType)
					{
						%found = %tool;
					}
				}
			}
			%ammo = %item.ammo;
			if(%item.ammo $= "")
				%ammo = $Pref::Server::Ranged::Ammo[%data.ammoType];
			if(%obj.ammo[%data.ammoType] >= $Pref::Server::Ranged::Ammo[%data.ammoType])
				return !%found ? parent::pickUp(%obj, %item) : 0;

			if(%ammo > 0 && %obj.ammo[%data.ammoType] <= 0 && isObject(%img = %obj.getMountedImage(0)) && isObject(%found) && %img.item.getID() == %found.getID())
				%obj.setImageLoaded(0, 1);

			%obj.ammo[%data.ammoType] = getMin($Pref::Server::Ranged::Ammo[%data.ammoType], %obj.ammo[%data.ammoType] + %ammo);
			%obj.updateDisplayAmmo(0, %data.ammoType);
			if(%ammo <= 0)
				return parent::pickUp(%obj, %item);

			if(%found)
			{
				if(isObject(%item.spawnBrick))
				{
					%item.fadeOut();
					%item.schedule(%item.spawnBrick.itemRespawnTime, fadeIn);
				}
				else
					%item.delete();
				return;
			}
		}
		parent::pickUp(%obj, %item);
	}

	function ItemData::onAdd(%this, %obj)
	{
		Parent::onAdd(%this, %obj);

		if ($DroppedAmmo !$= "")
		{
			%obj.ammo = $DroppedAmmo;
			$DroppedAmmo = "";
		}
	}

	function serverCmdDropTool(%client, %index)
	{
		%player = %client.player;

		$DroppedAmmo = %player.ammo[%player.tool[%index].ammoType];
		%player.ammo[%player.tool[%index].ammoType] = "";

		Parent::serverCmdDropTool(%client, %index);

		$DroppedAmmo = "";
	}
};
activatePackage(RPRangedPackage);