

/*24-08-2019 by Sajid Khan*/
ALTER TABLE `vendor` ADD COLUMN `has_note` INT NULL AFTER `logo`; 

ALTER TABLE `purchase_orders` ADD COLUMN `has_note` INT NULL AFTER `ra_flag`, ADD COLUMN `has_doc` INT NULL AFTER `has_note`;

