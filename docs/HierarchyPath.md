# KMD Logic Document Generation Api Client

## Hierarchy Path

Templates for a Logic Document Generation configuration are uploaded to template storage areas.
The supported storage areas and their respective configuration are elaborated upon [here](./Configuration/Configuration.md).

* A Logic Document Generation configuration can have many storage areas.

* Storage areas are configured into a tree or hierarchy.

* Each storage area has a string `key` and a single parent.

* The `key` of a storage area is unique within a set of sibling storage areas.

* The storage area that is the root (or master) node of the tree has an empty string key.

* Each storage area can be identified by a **hierarchy path** starting at the root (or master).

Where document generation client methods expect a `hierarchyPath` parameter (e.g. `RequestDocumentGeneration`, `GetTemplates`), the value provided needs to be encoded into a _path_ of keys, an encoding that is backslash separated.

Because the master storage area always has an empty key, it doesn't appear explicitly in the encoded hierarchy path.

For example, let's say you have a configuration with customers at level 1 and departments at level 2,  and you have a customer with key **A0001** with a department with key **B0001**.

Then the hierarchy path down to the department would be **\\A0001\\B0001\\**, the hierarchy path down to the customer would be **\\A0001\\**, and the hierarchy path identifying only the root or master storage area would be **\\**.

Hierarchy paths encoded in this way are displayed with each storage area at the Document Generation Configuration in Console.
