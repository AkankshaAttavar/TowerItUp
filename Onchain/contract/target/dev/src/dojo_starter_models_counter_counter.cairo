impl CounterIntrospect<> of dojo::database::introspect::Introspect<Counter<>> {
    #[inline(always)]
    fn size() -> Option<usize> {
        Option::Some(1)
    }

    fn layout() -> dojo::database::introspect::Layout {
        dojo::database::introspect::Layout::Struct(
            array![
                dojo::database::introspect::FieldLayout {
                    selector: 223925651276572801467025322450506123433664924558092583619131301620304795732,
                    layout: dojo::database::introspect::Introspect::<u32>::layout()
                }
            ]
                .span()
        )
    }

    #[inline(always)]
    fn ty() -> dojo::database::introspect::Ty {
        dojo::database::introspect::Ty::Struct(
            dojo::database::introspect::Struct {
                name: 'Counter',
                attrs: array![].span(),
                children: array![
                    dojo::database::introspect::Member {
                        name: 'entityId',
                        attrs: array!['key'].span(),
                        ty: dojo::database::introspect::Introspect::<u32>::ty()
                    },
                    dojo::database::introspect::Member {
                        name: 'counter',
                        attrs: array![].span(),
                        ty: dojo::database::introspect::Introspect::<u32>::ty()
                    }
                ]
                    .span()
            }
        )
    }
}

impl CounterModel of dojo::model::Model<Counter> {
    fn entity(
        world: dojo::world::IWorldDispatcher,
        keys: Span<felt252>,
        layout: dojo::database::introspect::Layout
    ) -> Counter {
        let values = dojo::world::IWorldDispatcherTrait::entity(
            world,
            1645666093578945487482204576769833870273866354439302355155481950661848817950,
            keys,
            layout
        );

        // TODO: Generate method to deserialize from keys / values directly to avoid
        // serializing to intermediate array.
        let mut serialized = core::array::ArrayTrait::new();
        core::array::serialize_array_helper(keys, ref serialized);
        core::array::serialize_array_helper(values, ref serialized);
        let mut serialized = core::array::ArrayTrait::span(@serialized);

        let entity = core::serde::Serde::<Counter>::deserialize(ref serialized);

        if core::option::OptionTrait::<Counter>::is_none(@entity) {
            panic!(
                "Model `Counter`: deserialization failed. Ensure the length of the keys tuple is matching the number of #[key] fields in the model struct."
            );
        }

        core::option::OptionTrait::<Counter>::unwrap(entity)
    }

    #[inline(always)]
    fn name() -> ByteArray {
        "Counter"
    }

    #[inline(always)]
    fn version() -> u8 {
        1
    }

    #[inline(always)]
    fn selector() -> felt252 {
        1645666093578945487482204576769833870273866354439302355155481950661848817950
    }

    #[inline(always)]
    fn instance_selector(self: @Counter) -> felt252 {
        Self::selector()
    }

    #[inline(always)]
    fn keys(self: @Counter) -> Span<felt252> {
        let mut serialized = core::array::ArrayTrait::new();
        core::serde::Serde::serialize(self.entityId, ref serialized);
        core::array::ArrayTrait::span(@serialized)
    }

    #[inline(always)]
    fn values(self: @Counter) -> Span<felt252> {
        let mut serialized = core::array::ArrayTrait::new();
        core::serde::Serde::serialize(self.counter, ref serialized);
        core::array::ArrayTrait::span(@serialized)
    }

    #[inline(always)]
    fn layout() -> dojo::database::introspect::Layout {
        dojo::database::introspect::Introspect::<Counter>::layout()
    }

    #[inline(always)]
    fn instance_layout(self: @Counter) -> dojo::database::introspect::Layout {
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
trait Icounter<T> {
    fn ensure_abi(self: @T, model: Counter);
}

#[starknet::contract]
mod counter {
    use super::Counter;
    use super::Icounter;

    #[storage]
    struct Storage {}

    #[abi(embed_v0)]
    impl DojoModelImpl of dojo::model::IModel<ContractState> {
        fn selector(self: @ContractState) -> felt252 {
            dojo::model::Model::<Counter>::selector()
        }

        fn name(self: @ContractState) -> ByteArray {
            dojo::model::Model::<Counter>::name()
        }

        fn version(self: @ContractState) -> u8 {
            dojo::model::Model::<Counter>::version()
        }

        fn unpacked_size(self: @ContractState) -> Option<usize> {
            dojo::database::introspect::Introspect::<Counter>::size()
        }

        fn packed_size(self: @ContractState) -> Option<usize> {
            dojo::model::Model::<Counter>::packed_size()
        }

        fn layout(self: @ContractState) -> dojo::database::introspect::Layout {
            dojo::model::Model::<Counter>::layout()
        }

        fn schema(self: @ContractState) -> dojo::database::introspect::Ty {
            dojo::database::introspect::Introspect::<Counter>::ty()
        }
    }

    #[abi(embed_v0)]
    impl counterImpl of Icounter<ContractState> {
        fn ensure_abi(self: @ContractState, model: Counter) {}
    }
}
