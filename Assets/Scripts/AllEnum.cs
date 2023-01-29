public enum EnumBlocks
{
    block,

    earth,
    backGroundEarth,
    coal,

    rock,
    backGroundRock,
    iron,

    rockFire,
    backGroundFire,
    emerald,

    rockIce,
    backGroundIce,
    amethyst
}

public enum EnumTools
{
    pickaxe,
    sword,
    axe
}

public enum EnumUI
{
    slotEquip
}

public enum EnumBiomes
{
    earth,
    earthRock,

    rock,
    rockIce,
    rockFire,

    ice,

    fire,
}

public enum EnumMaps
{
    caveGeneratorGame,
    caveGeneratorTest
}

public enum EnumState
{
    none,

    //player
    playerSpawn,
    playerMove,
    playerJump,
    playerEquipInventory,
    playerPickaxe,

    //UI
    UIEquipInventory
}