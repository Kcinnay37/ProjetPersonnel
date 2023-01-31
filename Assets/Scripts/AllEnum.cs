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

public enum EnumStatesMap
{
    generate,
    manager,
    view,
    data
}

public enum EnumStates
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

public enum EnumStateMachines
{
    stateMachineMap,
    stateMachinePlayer,
    stateMachineManager
}