// Generated by dojo-bindgen on Sat, 22 Jun 2024 20:40:02 +0000. Do not modify this file manually.
using System;
using Dojo;
using Dojo.Starknet;

// Type definition for `core::byte_array::ByteArray` struct
[Serializable]
public struct ByteArray {
    public string[] data;
    public FieldElement pending_word;
    public uint pending_word_len;
}

// Type definition for `core::option::Option::<core::integer::u32>` enum
public abstract record Option<A>() {
    public record Some(A value) : Option<A>;
    public record None() : Option<A>;
}


// Model definition for `dojo_starter::models::counter::Counter` model
public class Counter : ModelInstance {
    [ModelField("entityId")]
    public uint entityId;

    [ModelField("counter")]
    public uint counter;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
        