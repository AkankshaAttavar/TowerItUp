use starknet::ContractAddress;

#[derive(Copy, Drop, Serde)]
#[dojo::model]
struct Player {
    #[key]
    player: ContractAddress,
    score: u64
}
