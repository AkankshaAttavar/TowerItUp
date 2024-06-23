#[starknet::contract]
mod actions {
    use dojo::world;
    use dojo::world::IWorldDispatcher;
    use dojo::world::IWorldDispatcherTrait;
    use dojo::world::IWorldProvider;
    use dojo::world::IDojoResourceProvider;


    component!(
        path: dojo::components::upgradeable::upgradeable,
        storage: upgradeable,
        event: UpgradeableEvent
    );

    #[abi(embed_v0)]
    impl DojoResourceProviderImpl of IDojoResourceProvider<ContractState> {
        fn dojo_resource(self: @ContractState) -> felt252 {
            'actions'
        }
    }

    #[abi(embed_v0)]
    impl WorldProviderImpl of IWorldProvider<ContractState> {
        fn world(self: @ContractState) -> IWorldDispatcher {
            self.world_dispatcher.read()
        }
    }

    #[abi(embed_v0)]
    impl UpgradableImpl =
        dojo::components::upgradeable::upgradeable::UpgradableImpl<ContractState>;

    use super::{IActions};
    use starknet::{ContractAddress, get_caller_address, contract_address_const};
    use dojo_starter::models::{
        health::Health, player::{Player}, block::{Block},
        game::{Game, GameStatus, GameStatusImplTrait}, counter::{Counter}
    };


    const COUNTER_ID: u32 = 999999999;


    #[abi(embed_v0)]
    impl ActionsImpl of IActions<ContractState> {
        fn spawn(self: @ContractState) {
            let world = self.world_dispatcher.read();
            // Get the address of the current caller, possibly the player's address.
            let player = get_caller_address();

            let playerStatus = get!(world, player, (Player));

            let currentCounter = get!(world, COUNTER_ID, (Counter));
            let gameCounter = currentCounter.counter + 1;

            // spawn player character
            set!(
                world,
                (
                    Player { player, score: playerStatus.score },
                    Game { player, entityId: gameCounter, status: GameStatus::InProgress },
                    Health { entityId: player, gameId: gameCounter, health: 3 },
                    Block { gameId: gameCounter, x: 0, y: 0 },
                )
            );

            set!(world, (Counter { entityId: COUNTER_ID, counter: gameCounter }));
        }

        fn place_tile(self: @ContractState, x: u16, y: u16) {
            let world = self.world_dispatcher.read();
            let player = get_caller_address();
            let mut gameStatus = get!(world, player, (Game));

            let (mut playerCharacter, mut gameStatus) = get!(world, player, (Player, Game));
            let gameCounter = gameStatus.entityId;

            gameStatus.assert_in_progress();

            let mut score = playerCharacter.score;
            let mut health_ = get!(world, player, (Health));

            if (health_.health == 0) {
                gameStatus.status = GameStatus::Over;
                set!(world, (gameStatus));
            }

            if (score < 1) {
                set!(
                    world,
                    (
                        Player { player, score: score + 10 },
                        Game { player, entityId: gameCounter, status: GameStatus::InProgress },
                        Health { entityId: player, gameId: gameCounter, health: health_.health },
                        Block { gameId: gameCounter, x: x, y: y },
                    )
                );
            } else {
                let blockValue = get!(world, gameCounter, (Block));
                if (blockValue.x == x && blockValue.y == y) {
                    set!(
                        world,
                        (
                            Player { player, score: score + 30 },
                            Game { player, entityId: gameCounter, status: GameStatus::InProgress },
                            Health {
                                entityId: player, gameId: gameCounter, health: health_.health
                            },
                            Block { gameId: gameCounter, x: x, y: y },
                        )
                    );
                } else if (blockValue.x >= x
                    - 5 && blockValue.y >= y || blockValue.x <= x
                    - 5 && blockValue.y <= y) {
                    set!(
                        world,
                        (
                            Player { player, score: score + 10 },
                            Game { player, entityId: gameCounter, status: GameStatus::InProgress },
                            Health {
                                entityId: player, gameId: gameCounter, health: health_.health
                            },
                            Block { gameId: gameCounter, x: x, y: y },
                        )
                    );
                } else {
                    set!(
                        world,
                        (
                            Player { player, score: score },
                            Game { player, entityId: gameCounter, status: GameStatus::InProgress },
                            Health {
                                entityId: player, gameId: gameCounter, health: health_.health - 1
                            },
                            Block { gameId: gameCounter, x: x, y: y },
                        )
                    );
                }
            }
        }
    }

    #[starknet::interface]
    trait IDojoInit<ContractState> {
        fn dojo_init(self: @ContractState);
    }

    #[abi(embed_v0)]
    impl IDojoInitImpl of IDojoInit<ContractState> {
        fn dojo_init(self: @ContractState) {
            assert(
                starknet::get_caller_address() == self.world().contract_address,
                'Only world can init'
            );
        }
    }

    #[event]
    #[derive(Drop, starknet::Event)]
    enum Event {
        UpgradeableEvent: dojo::components::upgradeable::upgradeable::Event,
    }

    #[storage]
    struct Storage {
        world_dispatcher: IWorldDispatcher,
        #[substorage(v0)]
        upgradeable: dojo::components::upgradeable::upgradeable::Storage,
    }
}

