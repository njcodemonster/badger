

/*24-08-2019 by Sajid Khan*/
ALTER TABLE `vendor` ADD COLUMN `has_note` INT NULL AFTER `logo`; 

ALTER TABLE `purchase_orders` ADD COLUMN `has_note` INT NULL AFTER `ra_flag`, ADD COLUMN `has_doc` INT NULL AFTER `has_note`;


/*26-08-2019 by Sajid Khan*/
ALTER TABLE `purchase_orders` CHANGE `total_quantity` `total_quantity` DECIMAL(10,2) NOT NULL, CHANGE `subtotal` `subtotal` DECIMAL(10,2) NOT NULL, CHANGE `shipping` `shipping` DECIMAL(10,2) NOT NULL, CHANGE `po_discount_id` `po_discount_id` INT NULL; 

/*29-08-2019 by Sajid Khan*/
ALTER TABLE `items` ADD COLUMN `has_doc` INT NULL AFTER `published_by`;



/*30-08-2019 by Sajid Khan*/
INSERT INTO `event_types` (`event_type_name`, `event_type_description`) VALUES ('purchase order line item created', 'A new purchase order line item is created via api'); 
UPDATE `event_types` SET `event_type_name` = 'purchase order line item updated' WHERE `event_type_id` = '39'; 
UPDATE `event_types` SET `event_type_description` = 'A purchase order line item is updated via API' WHERE `event_type_id` = '39'; 
INSERT INTO `event_types` (`event_type_name`) VALUES ('purchase order line item specific updated'); 
UPDATE `event_types` SET `event_type_description` = 'A purchase order line item is specific updated via API' WHERE `event_type_id` = '40'; 


/*02-09-2019 by Sajid Khan*/
UPDATE `document_type` SET `doc_type` = 'Original PO' WHERE `doc_type_id` = '4'; 
INSERT INTO `document_type` (`doc_type`) VALUES ('Shipment Invoice'); 
INSERT INTO `document_type` (`doc_type`) VALUES ('Main Shipment Invoice'); 
INSERT INTO `document_type` (`doc_type`) VALUES ('Other'); 

ALTER TABLE `purchase_orders` CHANGE `vendor_order_number` `vendor_order_number` VARCHAR(150) NOT NULL; 