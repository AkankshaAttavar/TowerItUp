impl BlockIntrospect<> of dojo::database::introspect::Introspect<Block<>> {
    #[inline(always)]
    fn size() -> Option<usize> {
        Option::Some(2)
    }

    fn layout() -> dojo::database::introspect::Layout {
        dojo::database::introspect::Layout::Struct(
            array![
                dojo::database::introspect::FieldLayout {
                    selector: 512066735765477566404754172672287371265995314501343422459174036873487219331,
                    layout: dojo::database::introspect::Introspect::<u16>::layout()
                },
                dojo::database::introspect::FieldLayout {
                    selector: 1591024729085637502504777720563487898377940395575083379770417352976841400819,
                    layout: dojo::database::introspect::Introspect::<u16>::layout()
                }
            ]
                .span()
        )
    }

    #[inline(always)]
    fn ty() -> dojo::database::introspect::Ty {
        dojo::database::introspect::Ty::Struct(
            dojo::database::introspect::Struct {
                name: 'Block',
                attrs: array![].span(),
                children: array![
                    dojo::database::introspect::Member {
                        name: 'gameId',
                        attrs: array!['key'].span(),
                        ty: dojo::database::introspect::Introspect::<u32>::ty()
                    },
                    dojo::database::introspect::Member {
                        name: 'x',
                        attrs: array![].span(),
                        ty: dojo::database::introspect::Introspect::<u16>::ty()
                    },
                    dojo::database::introspect::Member {
                        name: 'y',
                        attrs: array![].span(),
                        ty: dojo::database::introspect::Introspect::<u16>::ty()
                    }
                ]
                    .span()
            }
        )
    }
}

impl BlockModel of dojo::model::Model<Block> {
    fn entity(
        world: dojo::world::IWorldDispatcher,
        keys: Span<felt252>,
        layout: dojo::database::introspect::Layout
    ) -> Block {
        let values = dojo::world::IWorldDispatcherTrait::entity(
            world,
            764063555842972967637158044376004476706732289798245498660561759612305598663,
            keys,
            layout
        );

        // TODO: Generate method to deserialize from keys / values directly to avoid
        // serializing to intermediate array.
        let mut serialized = core::array::ArrayTrait::new();
        core::array::serialize_array_helper(keys, ref serialized);
        core::array::serialize_array_helper(values, ref serialized);
        let mut serialized = core::array::ArrayTrait::span(@serialized);

        let entity = core::serde::Serde::<Block>::deserialize(ref serialized);

        if core::option::OptionTrait::<Block>::is_none(@entity) {
            panic!(
                "Model `Block`: deserialization failed. Ensure the length of the keys tuple is matching the number of #[key] fields in the model struct."
            );
        }

        core::option::OptionTrait::<Block>::unwrap(entity)
    }

    #[inline(always)]
    fn name() -> ByteArray {
        "Block"
    }

    #[inline(always)]
    fn version() -> u8 {
        1
    }

    #[inline(always)]
    fn selector() -> felt252 {
        764063555842972967637158044376004476706732289798245498660561759612305598663
    }

    #[inline(always)]
    fn instance_selector(self: @Block) -> felt252 {
        Self::selector()
    }

    #[inline(always)]
    fn keys(self: @Block) -> Span<felt252> {
        let mut serialized = core::array::ArrayTrait::new();
        core::serde::Serde::serialize(self.gameId, ref serialized);
        core::array::ArrayTrait::span(@serialized)
    }

    #[inline(always)]
    fn values(self: @Block) -> Span<felt252> {
        let mut serialized = core::array::ArrayTrait::new();
        core::serde::Serde::serialize(self.x, ref serialized);
        core::serde::Serde::serialize(self.y, ref serialized);
        core::array::ArrayTrait::span(@serialized)
    }

    #[inline(always)]
    fn layout() -> dojo::database::introspect::Layout {
        dojo::database::introspect::Introspect::<Block>::layout()
    }

    #[inline(always)]
    fn instance_layout(self: @Block) -> dojo::database::introspect::Layout {
        Self::layout()
    }

    #[inline(always)]
    fn packed_size() -> Option<usize> {
        let layout = Self::layout();

        match layout {
            dojo::database::introspect::Layout::Fixed(layout) => {
                let mut span_layout = layout;
                Option::Some(dojo::packing::calculate_packed_size(ref span_layout))
            },
            dojo::database::introspect::Layout::Struct(_) => Option::None,
            dojo::database::introspect::Layout::Array(_) => Option::None,
            dojo::database::introspect::Layout::Tuple(_) => Option::None,
            dojo::database::introspect::Layout::Enum(_) => Option::None,
            dojo::database::introspect::Layout::ByteArray => Option::None,
        }
    }
}

#[starknet::interface]
trait Iblock<T> {
    fn ensure_abi(self: @T, model: Block);
}

#[starknet::contract]
mod block {
    use super::Block;
    use super::Iblock;

    #[storage]
    struct Storage {}

    #[abi(embed_v0)]
    impl DojoModelImpl of dojo::model::IModel<ContractState> {
        fn selector(self: @ContractState) -> felt252 {
            dojo::model::Model::<Block>::selector()
        }

        fn name(self: @ContractState) -> ByteArray {
            dojo::model::Model::<Block>::name()
        }

        fn version(self: @ContractState) -> u8 {
            dojo::model::Model::<Block>::version()
        }

        fn unpacked_size(self: @ContractState) -> Option<usize> {
            dojo::database::introspect::Introspect::<Block>::size()
        }

        fn packed_size(self: @ContractState) -> Option<usize> {
            dojo::model::Model::<Block>::packed_size()
        }

        fn layout(self: @ContractState) -> dojo::database::introspect::Layout {
            dojo::model::Model::<Block>::layout()
        }

        fn schema(self: @ContractState) -> dojo::database::introspect::Ty {
            dojo::database::introspect::Introspect::<Block>::ty()
        }
    }

    #[abi(embed_v0)]
    impl blockImpl of Iblock<ContractState> {
        fn ensure_abi(self: @ContractState, model: Block) {}
    }
}
