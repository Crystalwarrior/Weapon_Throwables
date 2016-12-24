datablock AudioProfile(knifeThrow1)
{
	filename	= "./sounds/dagger_fly1.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(knifeThrow2)
{
	filename	= "./sounds/dagger_fly2.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(knifeSpring)
{
	filename	= "./sounds/knife_spring_01.wav";
	description	= AudioClosest3d;
	preload		= true;
};

datablock AudioProfile(knifeGore1)
{
	filename	= "./sounds/thrown_dagger_gore_01.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(knifeGore2)
{
	filename	= "./sounds/thrown_dagger_gore_02.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(knifeGore3)
{
	filename	= "./sounds/thrown_dagger_gore_03.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(knifeGore4)
{
	filename	= "./sounds/thrown_dagger_gore_04.wav";
	description	= AudioClosest3d;
	preload		= true;
};
datablock AudioProfile(knifeGore5)
{
	filename	= "./sounds/thrown_dagger_gore_05.wav";
	description	= AudioClosest3d;
	preload		= true;
};

datablock ExplosionData(ThrowKnifeStickExplosion : arrowStickExplosion)
{
   //explosionShape = "";
	soundProfile = knifeSpring;

	lifeTimeMS = 150;

	particleEmitter = arrowStickExplosionEmitter;
	particleDensity = 5;
	particleRadius = 0.1;

	emitter[0] = "";

	faceViewer     = true;
	explosionScale = "1 1 1";
};

datablock ProjectileData(ThrowKnifeProjectile)
{
	projectileShapeName		= "./throwingknife_proj.dts";
	directDamage			= 20;
	directDamageType		= $DamageType::Direct;
	radiusDamageType		= $DamageType::Direct;
	stickExplosion			= ThrowKnifeStickExplosion;
	bloodExplosion			= MeleeBloodExplosion;
	explosion				= arrowExplosion;
	muzzleVelocity			= 65;
	velInheritFactor		= 1;
	explodeOnPlayerImpact	= true;
	explodeOnDeath			= true;

	armingDelay				= 32000;
	lifetime				= 32000;
	fadeDelay				= 32000;
	isBallistic				= true;
	bounceAngle				= 170; //stick almost all the time
	minStickVelocity		= 10;
	bounceElasticity		= 0.2;
	bounceFriction			= 0.01;   
	gravityMod				= 1.0;

	hasLight				= false;

	uiName = "Thrown Knife";
};

//////////
// item //
//////////
datablock ItemData(ThrowKnifeItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./throwingknife.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Throwing Knife";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	 // Dynamic properties defined by the scripts
	image = ThrowKnifeImage;
	canDrop = true;
};

//function spear::onUse(%this,%user)
//{
//    //mount the image in the right hand slot
//    %user.mountimage(%this.image, $RightHandSlot);
//}

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(ThrowKnifeImage)
{
	// Basic Item properties
	shapeFile = "./throwingknife.dts";
	emap = true;

	isChargeWeapon = true; //Bot Hole support

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	offset = "0 0 0";
	rotation = eulerToMatrix("0 0 0");
	//eyeOffset = "0.1 0.2 -0.55";

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.  
	correctMuzzleVector = true;

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = ThrowKnifeItem;
	ammo = " ";
	projectile = ThrowKnifeProjectile;
	projectileType = Projectile;

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = false;

	//casing = " ";
	doColorShift = true;
	colorShiftColor = ThrowKnifeItem.colorShiftColor;

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.1;
	stateTransitionOnTimeout[0]		= "Ready";
	stateSound[0]					= "";

	stateName[1]					= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]		= true;
	stateScript[1]					= "onReady";
	
	stateName[2]					= "Charge";
	stateTransitionOnTimeout[2]		= "Armed";
	stateTimeoutValue[2]			= 0.5;
	stateWaitForTimeout[2]			= false;
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	stateAllowImageChange[2]		= false;
	stateSequence[2]				= "equip";
	stateScript[2]					= "onCharging";
	
	stateName[3]					= "AbortCharge";
	stateTransitionOnTimeout[3]		= "Ready";
	stateTimeoutValue[3]			= 0.1;
	stateWaitForTimeout[3]			= true;
	stateScript[3]					= "onAbortCharge";
	stateAllowImageChange[3]		= false;

	stateName[4]					= "Armed";
	stateTransitionOnTriggerUp[4]	= "Fire";
	stateAllowImageChange[4]		= false;
	stateScript[4]					= "onCharge";

	stateName[5]					= "Fire";
	stateTransitionOnTimeout[5]		= "Ready";
	stateTimeoutValue[5]			= 0.5;
	stateFire[5]					= true;
	stateSequence[5]				= "thrown";
	stateScript[5]					= "onFire";
	stateWaitForTimeout[5]			= true;
	stateAllowImageChange[5]		= false;
};

function ThrowKnifeImage::onMount(%this, %obj, %slot)
{
	parent::onMount(%this, %obj, %slot);
	fixArmReady(%obj);
}

function ThrowKnifeImage::onCharge(%this, %obj, %slot)
{
	if(%obj.getDatablock().shapeFile !$= "base/data/shapes/player/mmelee.dts")
		%obj.playthread(2, armReadyRight);
	%obj.playThread(3, plant);
	serverPlay3D(MeleeChargeSound, %obj.getSlotTransform(%slot));
}

function ThrowKnifeImage::onCharging(%this, %obj, %slot)
{
	if(%obj.getDatablock().shapeFile $= "base/data/shapes/player/mmelee.dts")
		%obj.playthread(2, tswing3);
	else
		%obj.playthread(2, rotCCW);
}

function ThrowKnifeImage::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, shiftRight);
}

function ThrowKnifeImage::onReady(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

function ThrowKnifeImage::onFire(%this, %obj, %slot)
{
	if(%obj.getDatablock().shapeFile $= "base/data/shapes/player/mmelee.dts")
	{
		%obj.playthread(2, tswing4);
		%obj.playThread(3, shiftDown);
	}
	else
		%obj.playthread(3, SpearThrow);
	serverPlay3D(knifeThrow @ getRandom(1, 2), %obj.getSlotTransform(%slot));
	Parent::OnFire(%this, %obj, %slot);
}

function ThrowKnifeProjectile::damage(%this, %obj, %col, %fade, %pos, %normal)
{
	%damageType = %this.directDamageType;
	%damage = %this.directDamage;
	%sound = knifeGore @ getRandom(1, 5);
	if(%col.getRegion(%pos, true) $= "head")
	{
		%damage *= 2;
		%sound = spearGore @ getRandom(1, 3);
	}

	ServerPlay3D(%sound, %pos);
	%col.damage(%obj, %pos, %damage, %damageType);
}