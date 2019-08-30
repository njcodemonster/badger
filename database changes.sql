

/*24-08-2019 by Sajid Khan*/
ALTER TABLE `vendor` ADD COLUMN `has_note` INT NULL AFTER `logo`; 

ALTER TABLE `purchase_orders` ADD COLUMN `has_note` INT NULL AFTER `ra_flag`, ADD COLUMN `has_doc` INT NULL AFTER `has_note`;


/*26-08-2019 by Sajid Khan*/
ALTER TABLE `purchase_orders` CHANGE `total_quantity` `total_quantity` DECIMAL(10,2) NOT NULL, CHANGE `subtotal` `subtotal` DECIMAL(10,2) NOT NULL, CHANGE `shipping` `shipping` DECIMAL(10,2) NOT NULL, CHANGE `po_discount_id` `po_discount_id` INT NULL; 

/*29-08-2019 by Sajid Khan*/
ALTER TABLE `items` ADD COLUMN `has_doc` INT NULL AFTER `published_by`;