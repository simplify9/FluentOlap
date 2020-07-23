using SW.FluentOlap.Models;

namespace UtilityUnitTests.Data
{
    public static class TestTypeMaps
    {
        public readonly static TypeMap P1TypeMap = new TypeMap {
            ["parcel1level_id"] = new NodeProperties
            {
                SqlType = InternalType.STRING
            },
            ["parcel1level_location"] = new NodeProperties
            {
                SqlType = InternalType.INTEGER
            },
            ["parcel1level_count"] = new NodeProperties
            {
                SqlType = InternalType.INTEGER
            }
        };

        public readonly static TypeMap P2TypeMap = new TypeMap {
            ["parcel2level_id"] = new NodeProperties
            {
                SqlType = InternalType.STRING
            },
            ["parcel2level_location"] = new NodeProperties
            {
                SqlType = InternalType.INTEGER
            },
            ["parcel2level_count"] = new NodeProperties
            {
                SqlType = InternalType.INTEGER
            },
            ["parcel2level_shipper_name"] = new NodeProperties
            {
                SqlType = InternalType.STRING
            },
            ["parcel2level_shipper_origincountry"] = new NodeProperties
            {
                SqlType = InternalType.STRING
            },
            ["parcel2level_supplier_name"] = new NodeProperties
            {
                SqlType = InternalType.STRING
            },
            ["parcel2level_supplier_destinationcountry"] = new NodeProperties
            {
                SqlType = InternalType.STRING
            },
        };

        public readonly static TypeMap P3TypeMap = new TypeMap
        {
            ["parcel3level_id"] = new NodeProperties
            {
                SqlType = InternalType.INTEGER
            },
            ["parcel3level_reference"] = new NodeProperties
            {
                SqlType = InternalType.STRING
            },
            ["parcel3level_referencetoparcel2level"] = new NodeProperties
            {
                SqlType = InternalType.STRING
            }
        };


    }
}
