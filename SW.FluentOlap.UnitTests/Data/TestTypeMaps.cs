using SW.FluentOlap.Models;

namespace UtilityUnitTests.Data
{
    public static class TestTypeMaps
    {
        public readonly static TypeMap P1TypeMap = new TypeMap
        {
            ["parcel1level_id"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel1level_location"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcel1level_count"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcel1level_datetime"] = new NodeProperties("")
            {
                InternalType = InternalType.DATETIME
            }
        };

        public readonly static TypeMap P2TypeMap = new TypeMap
        {
            ["parcel2level_id"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel2level_location"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcel2level_shipper_name"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel2level_shipper_origincountry"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel2level_shipper2_name"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel2level_shipper2_origincountry"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel2level_supplier_name"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel2level_supplier_destinationcountry"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel2level_supplier_phone_number"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            }
        };

        public readonly static TypeMap P3TypeMap = new TypeMap
        {
            ["parcel3level_id"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcel3level_reference"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["parcel3level_referencetoparcel2level"] = new NodeProperties("")
            {
                NodeName = "parcel2level",
                ServiceName = "someservice",
                InternalType = InternalType.STRING
            }
        };

        public readonly static TypeMap IgnoreTestMap = new TypeMap
        {
            ["ignoretestmap_co2sm_name2"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
            ["ignoretestmap_complex_stringprop"] = new NodeProperties("")
            {
                InternalType = InternalType.STRING
            },
        };

        public readonly static TypeMap SelfReferenceTest = new TypeMap
        {
            ["parcelselfreference_id"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference_selfref_id"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference_selfref_deeper_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference_selfref_deeper_deeperselfref_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference_deeper_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference_deeper_deeperselfref_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
        };


        public readonly static TypeMap SelfReferenceTestDeep = new TypeMap
        {
            ["parcelselfreference2_id"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference2_selfref_id"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference2_selfref_deeper_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference2_selfref_deeper_deeperselfref_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference2_deeper_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference2_deeper_deeperselfref_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference2_deeper_deeperselfref_parcelselfref_id"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference2_deeper_parcelselfref_id"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
            ["parcelselfreference2_deeper_parcelselfref_deeper_number"] = new NodeProperties("")
            {
                InternalType = InternalType.INTEGER
            },
        };
    }
}