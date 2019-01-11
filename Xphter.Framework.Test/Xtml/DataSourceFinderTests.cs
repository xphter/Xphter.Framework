using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Xtml;

namespace Xphter.Framework.Xtml.Tests {
    [TestClass()]
    public class DataSourceFinderTests {
        [TestMethod()]
        public void FindTest() {
            Customer customer = new Customer();
            Order order = new Order {
                OwnerCustomer = customer,
            };
            OrderGoods goods = new OrderGoods {
                OwnerOrder = order,
            };
            OrderReady ready = new OrderReady();
            OrderReadyGoods readyGoods = new OrderReadyGoods {
                OwnerGoods = goods,
            };
            OrderWarehouse warehouse = new OrderWarehouse {
                OwnerOrder = order,
            };
            OrderWarehouseGoods warehouseGoods = new OrderWarehouseGoods {
                OwnerGoods = goods,
                OwnerWarehouse = warehouse,
            };
            NewOrderWarehouseGoods newWarehouseGoods = new NewOrderWarehouseGoods {
                OwnerGoods = goods,
                OwnerWarehouse = warehouse,
            };
            OrderDelivery delivery = new OrderDelivery {
                OwnerOrder = order,
            };
            OrderDeliveryGoods deliveryGoods = new OrderDeliveryGoods {
                OwnerGoods = goods,
                OwnerDelivery = delivery,
            };
            OrderPayment payment = new OrderPayment {
                OwnerOrder = order,
            };

            DataSourceFinder finder = DataSourceFinder.Instance;
            Assert.AreSame(customer, finder.Find<Customer>(warehouseGoods));
            Assert.AreSame(customer, finder.Find<Customer>(newWarehouseGoods));
            Assert.AreSame(customer, finder.Find<Customer>(deliveryGoods));
            Assert.AreSame(customer, finder.Find<Customer>(readyGoods));
            Assert.AreSame(customer, finder.Find<Customer>(payment));
            Assert.IsNull(finder.Find<Customer>(ready));
            Assert.IsNull(finder.Find<A>(new C()));
        }

        [DataSource]
        public class NewOrderWarehouseGoods : OrderWarehouseGoods {
        }

        [DataSource]
        public class OrderWarehouseGoods {
            public OrderGoods OwnerGoods {
                get;
                set;
            }

            public OrderWarehouse OwnerWarehouse {
                get;
                set;
            }
        }

        [DataSource]
        public class OrderWarehouse {
            public Order OwnerOrder {
                get;
                set;
            }
        }

        [DataSource]
        public class OrderReady {
        }

        [DataSource]
        public class OrderReadyGoods {
            public OrderGoods OwnerGoods {
                get;
                set;
            }

            public OrderReady OwnerReady {
                get;
                set;
            }
        }

        [DataSource]
        public class OrderDeliveryGoods {
            public OrderGoods OwnerGoods {
                get;
                set;
            }

            public OrderDelivery OwnerDelivery {
                get;
                set;
            }
        }

        [DataSource]
        public class OrderDelivery {
            public Order OwnerOrder {
                get;
                set;
            }
        }

        [DataSource]
        public class OrderGoods {
            public Order OwnerOrder {
                get;
                set;
            }
        }

        [DataSource]
        public class Order {
            public Customer OwnerCustomer {
                get;
                set;
            }
        }

        [DataSource]
        public class Customer {
        }

        [DataSource]
        public class OrderPayment {
            public Order OwnerOrder {
                get;
                set;
            }
        }

        [DataSource]
        public class A {
            public virtual B AssociatedB {
                get;
                set;
            }

            public virtual C AssociatedC {
                get;
                set;
            }

            public virtual A NextA {
                get;
                set;
            }
        }

        [DataSource]
        public class B : A {
            public virtual A OwnerA {
                get;
                set;
            }

            public virtual B NextB {
                get;
                set;
            }
        }

        [DataSource]
        public class C : B {
            public virtual B OwnerB {
                get;
                set;
            }

            public override A OwnerA {
                get {
                    return base.OwnerA;
                }
                set {
                    base.OwnerA = value;
                }
            }

            public virtual C NextC {
                get;
                set;
            }
        }
    }
}
