use starknet::ContractAddress;

#[derive(Copy, Drop, Serde)]
#[dojo::model]
struct Block {
    #[key]
    gameId: u32,
    x: u16,
    y: u16,
}

