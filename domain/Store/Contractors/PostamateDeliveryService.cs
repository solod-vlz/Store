using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Contractors
{
    public class PostamateDeliveryService : IDeliveryService
    {
        private static IReadOnlyDictionary<string, string> cities = new Dictionary<string, string>
        {
            {"1", "Moscow" },
            {"2", "St.Peterburg" },
        };

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> postamates =
            new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                {"1",

                new Dictionary<string, string>
                {
                    {"1", "Казанский вокзал"},
                    {"2", "Охотный ряд"},
                    {"3", "Савеловский рынок"},
                }

                },

                {"2",

                new Dictionary<string, string>
                {
                    {"4", "Московский вокзал"},
                    {"5", "Гостинный двор"},
                    {"6", "Петропавловская крепость"},
                }

                },
            };

        public string UniqueCode => "Postamate";

        public string Title => "Delivery to postamates in Moscow and St.Peterburg";

        public OrderDelivery GetDelivery(Form form)
        {
            if (form.UniqueCode != UniqueCode || !form.IsFinal)
                throw new InvalidOperationException("Invalid Form"); //TODO write tests

            var cityId = form.Fields.Single(field => field.Name == "city")
                                    .Value;
            var cityName = cities[cityId];

            var postamateId = form.Fields.Single(field => field.Name == "postamate")
                                    .Value;
            var postamateName = postamates[cityId][postamateId];

            var parameters = new Dictionary<string, string>
            {
                {nameof(cityId), cityId },
                {nameof(cityName), cityName },
                {nameof(postamateId), postamateId },
                {nameof(postamateName), postamateName },
            };

            var description = $"City: {cityName} /nPostamate: {postamateName}";

            return new OrderDelivery(UniqueCode, description, parameters);
        }

        public Form CreateForm(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //TODO: write tests

            return new Form(UniqueCode, order.Id, 1, false, new[]
            {
                new SelectionField("City", "city", "1", cities),
            });
        }

        public Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            if (step == 1)
            {
                if (values["city"] == "1")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("City", "city", "1"),
                        new SelectionField("Postamate", "postamate", "1", postamates["1"]),
                    });
                }
                else if (values["city"] == "2")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("City", "city", "2"),
                        new SelectionField("Postamate", "postamate", "4", postamates["2"]),
                    });
                }
                else
                    throw new InvalidOperationException("Invalid postamate city.");
            }
            else if (step == 2)
            {
                return new Form(UniqueCode, orderId, 3, true, new Field[]
                {
                    new HiddenField("City", "city", values["city"]),
                    new HiddenField("Postamate", "postamate", values["postamate"]),
                });
            }
            else
                throw new InvalidOperationException("Invalid postamate step.");
        }
    }
}
